using AutoMapper;
using Cinema.Core.Entites;
using Cinema.Data;
using Cinema.Data.Repositories.Implementations;
using Cinema.Data.Repositories.Interfaces;
using Cinema.Service.Dtos;
using Cinema.Service.Dtos.MovieDtos;
using Cinema.Service.Dtos.NewsDtos;
using Cinema.Service.Dtos.OrderDtos;
using Cinema.Service.Exceptions;
using Cinema.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Cinema.Service.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;


        public OrderService(AppDbContext context, IOrderRepository orderRepository, IMapper mapper)
        {
            _context = context;
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public PaginatedList<AdminOrderGetDto> GetAllByPage(int page = 1, int size = 10)
        {
            var query = _orderRepository.GetAll(x => true)
            .Include(order => order.User)
            .Include(order => order.Session)
                .ThenInclude(session => session.Language)
            .Include(order => order.Session)
                .ThenInclude(session => session.Movie)
            .Include(order => order.Session)
                .ThenInclude(session => session.Hall)
                    .ThenInclude(h => h.Branch)
            .Include(order => order.OrderSeats)
                .ThenInclude(os => os.Seat)
            .OrderByDescending(order => order.OrderDate);

            var paginated = PaginatedList<Order>.Create(query, page, size);
            var ordersDto = _mapper.Map<List<AdminOrderGetDto>>(paginated.Items);
            return new PaginatedList<AdminOrderGetDto>(ordersDto, paginated.TotalPages, page, size);
        }

        public async Task<List<OrderDetailDto>> GetAllOrderDetailsAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Session)
                    .ThenInclude(s => s.Movie)
                .Include(o => o.Session)
                    .ThenInclude(s => s.Hall)
                        .ThenInclude(h => h.Branch)
                .Include(o => o.Session)
                    .ThenInclude(s => s.Language)
                .Include(o => o.OrderSeats)
                    .ThenInclude(os => os.Seat)
                .Select(o => new OrderDetailDto
                {
                    UserName = o.User.UserName,
                    EmailOfUser = o.User.Email,
                    BranchName = o.Session.Hall.Branch.Name,
                    HallName = o.Session.Hall.Name, 
                    MovieName = o.Session.Movie.Title,
                    SessionDate = o.Session.ShowDateTime,
                    OrderDate = o.OrderDate,
                    Language = o.Session.Language.Name,
                    TotalPrice = o.TotalPrice,
                    SeatNumbers = o.OrderSeats.Select(os => os.Seat.Number).ToList()
                })
                .ToListAsync();

            return orders;
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

        public async Task<BookSeatResult> BookSeatsAsync(BookSeatDto bookSeatDto, string userId)
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
                UserId = userId,
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

        public async Task<decimal> GetDailyTotalPriceAsync()
        {
            var now = DateTime.Now;
            var startDate = now.Date;
            var endDate = startDate.AddDays(1).AddTicks(-1);

            var totalPrice = await _orderRepository.GetAll(x => x.OrderDate >= startDate && x.OrderDate <= endDate)
                                                   .SumAsync(x => x.TotalPrice);

            return totalPrice;
        }
    }
}
