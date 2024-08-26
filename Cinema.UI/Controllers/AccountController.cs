using Cinema.UI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using Cinema.UI.Exceptions;
using Cinema.UI.Models.UserModels;
using Cinema.UI.Services;
using System.Linq;
using Cinema.UI.Extensions;

namespace Cinema.UI.Controllers
{
    public class AccountController : Controller
    {
        private HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICrudService _crudService;

        public AccountController(ICrudService service, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _client = new HttpClient();
            _crudService = service;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            var content = new StringContent(JsonSerializer.Serialize(loginRequest, options), Encoding.UTF8, "application/json");
            using (var response = await _client.PostAsync("https://localhost:44324/api/admin/auth/login", content))
            {
                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = JsonSerializer.Deserialize<LoginResponse>(await response.Content.ReadAsStringAsync(), options);
                    if (loginResponse.Token.NeedsPasswordReset)
                    {
                        TempData["ResetUserName"] = loginRequest.UserName;
                        Response.Cookies.Append("token", "Bearer " + loginResponse.Token.Token);
                        TempData["Token"] = loginResponse.Token.Token;
                        _httpContextAccessor.HttpContext.Session.SetBool("NeedsPasswordReset", true);

                        return RedirectToAction("ResetPassword");
                    }
                    Response.Cookies.Append("token", "Bearer " + loginResponse.Token.Token);
                    _httpContextAccessor.HttpContext.Session.SetBool("NeedsPasswordReset", false);
                    return RedirectToAction("Index", "Home");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    ModelState.AddModelError("", "UserName or Password incorrect!");
                    return View();
                }
                else
                {
                    TempData["Error"] = "Something went wrong";
                }
            }

            return View();
        }



        public IActionResult ResetPassword()
        {
            var userName = TempData["ResetUserName"] as string;
            var token = TempData["Token"] as string;
            if (userName == null || !_httpContextAccessor.HttpContext.Session.GetBool("NeedsPasswordReset"))
            {
                return RedirectToAction("Login");
            }

            var model = new ResetPasswordViewModel
            {
                UserName = userName,
                Token = token
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _crudService.Update<ResetPasswordViewModel>(model, "auth/updatePassword");
                return RedirectToAction("Login");
            }
            catch (ModelException e)
            {
                foreach (var item in e.Error.Errors)
                {
                    ModelState.AddModelError(item.Key, item.Message);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error occured. Please try again.");
                return View(model);
            }
        }


        public IActionResult CreateAdminBySuperAdmin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdminBySuperAdmin(AdminCreateRequest createRequest)
        {
            if (!ModelState.IsValid)
                return View(createRequest);

            try
            {
                await _crudService.CreateAdmin(createRequest, "CreateAdmin");
                return RedirectToAction("ShowAdmins");
            }
            catch (ModelException e)
            {
                foreach (var item in e.Error.Errors)
                    ModelState.AddModelError(item.Key, item.Message);
                return View(createRequest);
            }
        }


        public async Task<IActionResult> ShowAdmins(int page = 1)
        {
            try
            {
                var paginatedResponse = await _crudService.GetAllPaginated<AdminGetResponse>("Auth/GetAllByPage", page);

                return View(paginatedResponse);

            }
            catch (HttpException ex)
            {

                if (ex.Status == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Auth");
                }

                return RedirectToAction("Error", "Home");
            }

            catch (System.Exception)
            {
                return RedirectToAction("Error", "Home");
            }
        }


        public async Task<IActionResult> Profile()
        {
            var user = await _crudService.Get<AdminGetResponse>("auth/profile");

            AdminProfileEditRequest adminProfile = new AdminProfileEditRequest
            {
                UserName = user.UserName
            };
            TempData["UserId"] = user.Id;

            if (adminProfile == null)
            {
                return NotFound();
            }
            return View(adminProfile);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(AdminProfileEditRequest editRequest, string id)
        {
            if (!ModelState.IsValid)
            {
                TempData["ProfileEditError"] = "Please try again.";
                TempData["UserId"] = id;
                return View(editRequest);
            }

            try
            {
                await _crudService.Update<AdminProfileEditRequest>(editRequest, "auth/update/" + id);

                if (Request.Cookies.ContainsKey("token"))
                {
                    Response.Cookies.Delete("token");
                }
                return RedirectToAction("Login", "Account");
            }
            catch (ModelException e)
            {
                TempData["UserId"] = id;
                foreach (var item in e.Error.Errors)
                {
                    TempData["ProfileUpdateError"] = "Please try again.";
                    ModelState.AddModelError(item.Key, item.Message);
                }

                return View(editRequest);
            }
        }
        public IActionResult GoogleLogin()
        {
            return View();
        }
        public IActionResult ExternalLoginCallback(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            return RedirectToAction("LoginSuccess", "Home");
        }
        public async Task<IActionResult> Logout()
        {

            if (Request.Cookies.ContainsKey("token"))
            {
                Response.Cookies.Delete("token");
            }

            return RedirectToAction("Login", "Account");
        }
    }
}
