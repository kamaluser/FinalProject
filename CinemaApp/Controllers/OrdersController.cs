using Cinema.Service.Dtos.MovieDtos;
using Cinema.Service.Dtos;
using Cinema.Service.Dtos.OrderDtos;
using Cinema.Service.Implementations;
using Cinema.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Cinema.UI.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CinemaApp.Controllers
{
    //[Authorize(Roles = "Admin, Superadmin")]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IHubContext<BookingHub> _hubContext;

        public OrdersController(IOrderService orderService, IHubContext<BookingHub> hubContext)
        {
            _orderService = orderService;
            _hubContext = hubContext;
        }

        [HttpGet("api/admin/orders/GetAllByPagination")]
        public ActionResult<PaginatedList<AdminOrderGetDto>> GetAllByPagination(int page = 1, int size = 6)
        {
            if (page <= 0) page = 1;
            if (size <= 0) size = 3;

            return StatusCode(200, _orderService.GetAllByPage(page, size));
        }


        [HttpGet("api/admin/orders/price/daily")]
        public async Task<IActionResult> GetDailyTotalPrice()
        {
            var totalPrice = await _orderService.GetDailyTotalPriceAsync();
            return Ok(new { totalPrice });
        }


        [HttpGet("api/admin/orders/monthly-count-current-year")]
        public async Task<IActionResult> GetMonthlyOrderCounts()
        {
            var counts = await _orderService.GetMonthlyOrderCountsForCurrentYearAsync();


            var response = new
            {
                months = counts.Keys.ToArray(),
                orders = counts.Values.ToArray()
            };

            return Ok(response);
        }


        [HttpGet("api/admin/orders/monthly-revenue-current-year")]
        public async Task<IActionResult> GetMonthlyRevenue()
        {
            var revenues = await _orderService.GetMonthlyRevenueForCurrentYearAsync();

            var response = new
            {
                months = revenues.Keys.ToArray(),
                revenue = revenues.Values.ToArray()
            };

            return Ok(response);
        }


        [HttpGet("api/admin/orders/order-count-last-month")]
        public async Task<IActionResult> GetOrderCountLastMonth()
        {
            var result = await _orderService.GetOrderCountLastMonthAsync();
            return Ok(result);
        }

        [HttpGet("api/admin/orders/order-count-last-year")]
        public async Task<IActionResult> GetOrderCountLastYear()
        {
            var result = await _orderService.GetOrderCountLastYearAsync();
            return Ok(result);
        }

        [HttpPost("book-seats")]
        public async Task<IActionResult> BookSeats([FromBody] BookSeatDto bookSeatDto)
        {
            var result = await _orderService.BookSeatsAsync(bookSeatDto);
            if (result.Success)
            {
                await _hubContext.Clients.All.SendAsync("ReceiveNotification", "A new seat has been booked!");
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("api/admin/orders/details")]
        public async Task<IActionResult> GetAllOrderDetails()
        {
            var orderDetails = await _orderService.GetAllOrderDetailsAsync();
            return Ok(orderDetails);
        }

    }
}
