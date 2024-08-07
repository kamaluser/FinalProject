using Cinema.Service.Dtos.OrderDtos;
using Cinema.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CinemaApp.Controllers
{
    //[Authorize(Roles = "Member")]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
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
