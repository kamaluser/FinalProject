using Cinema.Core.Entites;
using Cinema.Data;
using Cinema.Data.Repositories.Implementations;
using Cinema.Data.Repositories.Interfaces;
using Cinema.Service.Dtos;
using Cinema.Service.Dtos.OrderDtos;
using Cinema.Service.Exceptions;
using Cinema.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cinema.Service.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IOrderRepository _orderRepository;

        public OrderService(AppDbContext context, IOrderRepository orderRepository)
        {
            _context = context;
            _orderRepository = orderRepository;
        }
        public async Task<Dictionary<string, int>> GetMonthlyOrderCountsForCurrentYearAsync()
        {
            var firstDayOfYear = new DateTime(DateTime.Today.Year, 1, 1);
            var firstDayOfNextYear = firstDayOfYear.AddYears(1);

            var monthlyCounts = await _context.Orders
                .Where(a => a.OrderDate >= firstDayOfYear && a.OrderDate < firstDayOfNextYear)
                .GroupBy(a => a.OrderDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var result = new Dictionary<string, int>();
            var months = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

            foreach (var month in months.Select((name, index) => new { name, index }))
            {
                var count = monthlyCounts.FirstOrDefault(m => m.Month == month.index + 1)?.Count ?? 0;
                result[month.name] = count;
            }

            return result;
        }


        public async Task<Dictionary<string, decimal>> GetMonthlyRevenueForCurrentYearAsync()
        {
            var firstDayOfYear = new DateTime(DateTime.Today.Year, 1, 1);
            var firstDayOfNextYear = firstDayOfYear.AddYears(1);

            var monthlyRevenues = await _context.Orders
                .Where(a => a.OrderDate >= firstDayOfYear && a.OrderDate < firstDayOfNextYear)
                .GroupBy(a => a.OrderDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Revenue = g.Sum(a => a.TotalPrice) 
                })
                .ToListAsync();

            var result = new Dictionary<string, decimal>();
            var months = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

            foreach (var month in months.Select((name, index) => new { name, index }))
            {
                var revenue = monthlyRevenues.FirstOrDefault(m => m.Month == month.index + 1)?.Revenue ?? 0m;
                result[month.name] = revenue;
            }

            return result;
        }

        /*  public async Task<BookSeatResult> BookSeatsAsync(BookSeatDto bookSeatDto)
          {
              var session = await _context.Sessions
                  .Include(s => s.Hall)
                  .FirstOrDefaultAsync(s => s.Id == bookSeatDto.SessionId);

              if (session == null)
              {
                  throw new RestException(StatusCodes.Status404NotFound, "Session not found.");
              }

              var seats = await _context.Seats
                  .Include(s => s.OrderSeats)
                  .ThenInclude(os=>os.Order)
                  .Where(s => bookSeatDto.SeatIds.Contains(s.Id) && s.HallId == session.HallId)
                  .ToListAsync();

              if (seats.Count != bookSeatDto.SeatIds.Count)
              {
                  throw new RestException(StatusCodes.Status404NotFound, "Some seats not found.");
              }

              foreach (var seat in seats)
              {
                  var existingOrderSeat = seat.OrderSeats?.FirstOrDefault(os => os.Order != null && os.Order.SessionId == bookSeatDto.SessionId);


                  if (existingOrderSeat != null)
                  {
                      throw new RestException(StatusCodes.Status400BadRequest, "One or more seats are already reserved for this session.");
                  }
              }

              var order = new Order
              {
                  UserId = bookSeatDto.UserId,
                  SessionId = bookSeatDto.SessionId,
                  OrderDate = DateTime.Now,
                  NumberOfSeats = bookSeatDto.SeatIds.Count,
                  TotalPrice = session.Price * bookSeatDto.SeatIds.Count,
                  OrderSeats = bookSeatDto.SeatIds.Select(seatId => new OrderSeat { SeatId = seatId }).ToList()
              };

              foreach (var seat in seats)
              {
                  seat.OrderSeats.Add(new OrderSeat
                  {
                      SeatId = seat.Id,
                      Order = order
                  });
              }

              _context.Orders.Add(order);
              await _context.SaveChangesAsync();

              return new BookSeatResult { Success = true };
          }
  */

        public async Task<BookSeatResult> BookSeatsAsync(BookSeatDto bookSeatDto)
        {
            var session = await _context.Sessions
                .Include(s => s.Hall)
                .FirstOrDefaultAsync(s => s.Id == bookSeatDto.SessionId);

            if (session == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Session not found.");
            }

            var seats = await _context.Seats
                .Include(s => s.OrderSeats)
                .ThenInclude(os => os.Order)
                .Where(s => bookSeatDto.SeatIds.Contains(s.Id) && s.HallId == session.HallId)
                .ToListAsync();

            if (seats.Count != bookSeatDto.SeatIds.Count)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Some seats not found.");
            }

            foreach (var seat in seats)
            {
                var existingOrderSeat = seat.OrderSeats?.FirstOrDefault(os => os.Order != null && os.Order.SessionId == bookSeatDto.SessionId);

                if (existingOrderSeat != null)
                {
                    throw new RestException(StatusCodes.Status400BadRequest, "One or more seats are already reserved for this session.");
                }
            }

            var order = new Order
            {
                UserId = bookSeatDto.UserId,
                SessionId = bookSeatDto.SessionId,
                OrderDate = DateTime.Now,
                NumberOfSeats = bookSeatDto.SeatIds.Count,
                TotalPrice = session.Price * bookSeatDto.SeatIds.Count
            };

            var orderSeats = seats.Select(seat => new OrderSeat { SeatId = seat.Id, Order = order });
            _context.Set<OrderSeat>().AddRange(orderSeats);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return new BookSeatResult { Success = true };
        }
        public async Task<OrderCountDto> GetOrderCountLastMonthAsync()
        {
            var lastMonth = DateTime.Now.AddMonths(-1);
            var count = await _context.Orders
                .Where(o => o.OrderDate >= lastMonth)
                .CountAsync();

            return new OrderCountDto { Count = count };
        }

        public async Task<OrderCountDto> GetOrderCountLastYearAsync()
        {
            var lastYear = DateTime.Now.AddYears(-1);
            var count = await _context.Orders
                .Where(o => o.OrderDate >= lastYear)
                .CountAsync();

            return new OrderCountDto { Count = count };
        }

        public decimal GetMonthlyTotalPriceAsync()
        {
            var now = DateTime.UtcNow;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var orders = _orderRepository.GetAll(x=>x.OrderDate>=startDate && x.OrderDate<=endDate)
                                                    .ToList();

            decimal totalPrice = 0;

            foreach (var order in orders)
            {
                totalPrice += order.TotalPrice;
            }

            return totalPrice;
        }
    }
}
