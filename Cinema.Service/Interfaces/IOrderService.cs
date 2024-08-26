using Cinema.Service.Dtos;
using Cinema.Service.Dtos.OrderDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Interfaces
{
    public interface IOrderService
    {
        Task<BookSeatResult> BookSeatsAsync(BookSeatDto bookSeatDto);
        Task<OrderCountDto> GetOrderCountLastMonthAsync();
        Task<OrderCountDto> GetOrderCountLastYearAsync();
        Task<Dictionary<string, int>> GetMonthlyOrderCountsForCurrentYearAsync();
        Task<decimal> GetDailyTotalPriceAsync();
        Task<Dictionary<string, decimal>> GetMonthlyRevenueForCurrentYearAsync();
        Task<List<OrderDetailDto>> GetAllOrderDetailsAsync();
        PaginatedList<AdminOrderGetDto> GetAllByPage(int page = 1, int size = 10);
    }
}