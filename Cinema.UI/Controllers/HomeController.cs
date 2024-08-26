using Cinema.UI.Exceptions;
using Cinema.UI.Filters;
using Cinema.UI.Models;
using Cinema.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

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


        [HttpGet("api/languages/monthly-session-languages")]
        public async Task<IActionResult> SessionLanguages()
        {
            try
            {
                var sessionLanguages = await _crudService.GetSessionLanguagesAsync();

                var jsonResult = JsonSerializer.Serialize(sessionLanguages);
                Console.WriteLine(jsonResult);

                var desiredLanguages = new[] { "English", "Russian", "Azerbaijan", "Turkish" };
                var filteredLanguages = sessionLanguages
                    .Where(sl => desiredLanguages.Contains(sl.Language))
                    .Select(sl => new
                    {
                        Language = sl.Language,
                        SessionCount = sl.SessionCount
                    })
                    .ToList();

                return Ok(filteredLanguages);
            }
            catch (HttpException ex)
            {
                if (ex.Status == System.Net.HttpStatusCode.Unauthorized)
                {
                    return Unauthorized("You are not authorized.");
                }

                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
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

        [HttpGet("api/orders/daily-total-price")]
        public async Task<IActionResult> GetDailyTotalPrice()
        {
            try
            {
                var totalPrice = await _crudService.GetDailyTotalPriceAsync();
                return Ok(totalPrice);
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
