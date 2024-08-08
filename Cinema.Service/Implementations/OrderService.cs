using Cinema.Core.Entites;
using Cinema.Data;
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
                .Where(s => bookSeatDto.SeatIds.Contains(s.Id) && s.HallId == session.HallId)
                .ToListAsync();

            if (seats.Count != bookSeatDto.SeatIds.Count)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Some seats not found.");
            }

            foreach (var seat in seats)
            {
                if (seat.IsOrdered)
                {
                    throw new RestException(StatusCodes.Status400BadRequest, "One or more seats are already reserved.");
                }

                seat.IsOrdered = true;
                seat.BookedFrom = session.ShowDateTime;
                seat.BookedUntil = session.ShowDateTime.AddMinutes(session.Duration);
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

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return new BookSeatResult { Success = true };
        }


        public async Task ResetExpiredReservationsAsync()
        {
            var now = DateTime.Now;
            var seatsToReset = await _context.Seats
                .Where(s => s.IsOrdered && s.BookedUntil <= now)
                .ToListAsync();

            foreach (var seat in seatsToReset)
            {
                seat.IsOrdered = false;
                seat.BookedFrom = null;
                seat.BookedUntil = null;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<OrderStatisticsDto> GetOrderStatisticsAsync()
        {
            var now = DateTime.Now;
            var startDate = now.AddMonths(-1);

            var orders = await _context.Orders
                .Where(o => o.OrderDate >= startDate)
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new OrderStatsByDateDto
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .OrderBy(d => d.Date)
                .ToListAsync();

            return new OrderStatisticsDto
            {
                TotalOrders = await _context.Orders.CountAsync(),
                OrdersByDate = orders
            };
        }

        public async Task<int> GetTotalOrdersCountAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .CountAsync();
        }

        public async Task<decimal> GetTotalOrderPriceAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .SumAsync(o => o.TotalPrice);
        }

    }
}
