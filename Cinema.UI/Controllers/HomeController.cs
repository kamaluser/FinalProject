using Cinema.UI.Exceptions;
using Cinema.UI.Filters;
using Cinema.UI.Models;
using Cinema.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Cinema.UI.Controllers
{
    [ServiceFilter(typeof(AuthFilter))]
    public class HomeController : Controller
    {
        private readonly ICrudService _crudService;

        public HomeController(ICrudService crudService)
        {
            _crudService = crudService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult LoginSuccess()
        {
            return View();
        }

        [HttpGet("api/orders/last-month-count")]
        public async Task<IActionResult> GetOrderCountLastMonth()
        {
            try
            {
                var orderCount = await _crudService.GetOrderCountLastMonthAsync();
                return Ok(orderCount);
            }
            catch (HttpException ex)
            {
                if (ex.Status == System.Net.HttpStatusCode.Unauthorized)
                {
                    return Unauthorized("You are not authorized.");
                }

                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
            catch (System.Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
        [HttpGet("api/orders/price/monthly")]
        public async Task<IActionResult> GetMonthlyTotalPrice()
        {
            try
            {
                var totalPrice = await _crudService.GetMonthlyRevenueAsync();
                return Ok(new { totalPrice }); 
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpGet("api/account/members-count")]
        public async Task<IActionResult> GetMembersCount()
        {
            try
            {
                var memberCount = await _crudService.GetMembersCountAsync();
                return Ok(memberCount);
            }
            catch (HttpException ex)
            {
                if (ex.Status == System.Net.HttpStatusCode.Unauthorized)
                {
                    return Unauthorized("You are not authorized.");
                }

                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
            catch (System.Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }


        [HttpGet("api/orders/last-year-count")]
        public async Task<IActionResult> GetOrderCountLastYear()
        {
            try
            {
                var orderCount = await _crudService.GetOrderCountLastYearAsync();
                return Ok(orderCount);
            }
            catch (HttpException ex)
            {
                if (ex.Status == System.Net.HttpStatusCode.Unauthorized)
                {
                    return Unauthorized("You are not authorized.");
                }

                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
            catch (System.Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpGet("api/sessions/last-month-count")]
        public async Task<IActionResult> GetSessionCountLastMonth()
        {
            try
            {
                var sessionCount = await _crudService.GetSessionCountLastMonthAsync();
                return Ok(sessionCount);
            }
            catch (HttpException ex)
            {
                if (ex.Status == System.Net.HttpStatusCode.Unauthorized)
                {
                    return Unauthorized("You are not authorized.");
                }

                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
            catch (System.Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpGet("api/orders/monthly-count-current-year")]
        public async Task<IActionResult> GetMonthlyOrderCountForCurrentYear()
        {
            try
            {
                var monthlyOrderCounts = await _crudService.GetMonthlyOrderCountForCurrentYearAsync();

                if (monthlyOrderCounts == null || monthlyOrderCounts.Months == null || monthlyOrderCounts.Orders == null)
                {
                    return NotFound("No data available for the current year.");
                }

                return Ok(monthlyOrderCounts);
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return Unauthorized("You are not authorized.");
                }

                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while processing your request: {ex.Message}");
            }
        }

        [HttpGet("api/orders/monthly-revenue-current-year")]
        public async Task<IActionResult> GetMonthlyRevenueForCurrentYear()
        {
            try
            {
                var monthlyRevenue = await _crudService.GetMonthlyRevenueForCurrentYearAsync();

                if (monthlyRevenue == null)
                {
                    return NotFound("No data available for the current year.");
                }

                return Ok(monthlyRevenue);
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return Unauthorized("You are not authorized.");
                }

                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while processing your request: {ex.Message}");
            }
        }


        [HttpGet("Api/Seats/GetOrdered")]
        public async Task<IActionResult> GetOrderedSeats()
        {
            try
            {
                var orderedSeats = await _crudService.GetTotalOrderedSeatsCountAsync();
                return Ok(orderedSeats);
            }
            catch (HttpException ex)
            {
                if (ex.Status == System.Net.HttpStatusCode.Unauthorized)
                {
                    return Unauthorized("You are not authorized.");
                }

                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
            catch (System.Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
