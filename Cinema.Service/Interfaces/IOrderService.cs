using Cinema.Service.Dtos.OrderDtos;
using System;
using System.Threading.Tasks;

namespace Cinema.Service.Interfaces
{
    public interface IOrderService
    {
        Task<BookSeatResult> BookSeatsAsync(BookSeatDto bookSeatDto);
        Task ResetExpiredReservationsAsync();
        Task<OrderStatisticsDto> GetOrderStatisticsAsync();
        Task<int> GetTotalOrdersCountAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalOrderPriceAsync(DateTime startDate, DateTime endDate);
    }
}
