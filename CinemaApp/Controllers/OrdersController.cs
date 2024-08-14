using Cinema.Service.Dtos.OrderDtos;
using Cinema.Service.Implementations;
using Cinema.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CinemaApp.Controllers
{
    //[Authorize(Roles = "Admin, Superadmin")]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("api/admin/orders/price/monthly")]
        public IActionResult GetMonthlyTotalPrice()
        {
            var totalPrice = _orderService.GetMonthlyTotalPriceAsync();
            return Ok(new { totalPrice });
        }


        [HttpGet("api/admin/orders/total-ordered-seats-count")]
        public async Task<IActionResult> GetTotalOrderedSeatsCount()
        {
            try
            {
                var count = await _orderService.GetTotalOrderedSeatsCountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
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
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
