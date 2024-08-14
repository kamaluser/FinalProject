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
        Task ResetExpiredReservationsAsync();
        Task<OrderCountDto> GetOrderCountLastMonthAsync();
        Task<OrderCountDto> GetOrderCountLastYearAsync();
        Task<Dictionary<string, int>> GetMonthlyOrderCountsForCurrentYearAsync();
        Task<int> GetTotalOrderedSeatsCountAsync();
        decimal GetMonthlyTotalPriceAsync();
        Task<Dictionary<string, decimal>> GetMonthlyRevenueForCurrentYearAsync();
    }
}