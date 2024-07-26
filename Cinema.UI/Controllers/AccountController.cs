using Cinema.UI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;

namespace Cinema.UI.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _client;

        public AccountController(HttpClient client)
        {
            _client = client;
        }

        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest loginRequest, string returnUrl = null)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var content = new StringContent(JsonSerializer.Serialize(loginRequest, options), Encoding.UTF8, "application/json");

            using (var response = await _client.PostAsync("http://localhost:5194/api/admin/Auth/login", content))
            {
                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = JsonSerializer.Deserialize<LoginResponse>(await response.Content.ReadAsStringAsync(), options);
                    Response.Cookies.Append("token", loginResponse.Token);

                    if (returnUrl != null && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("index", "home");
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    ModelState.AddModelError("", "Username or Password is incorrect!");
                    ViewData["ReturnUrl"] = returnUrl;
                    return View();
                }
                else
                {
                    TempData["Error"] = "Something went wrong!";
                }
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
    }
}
