using Cinema.Core.Entites;
using Cinema.Service.Interfaces;
using Cinema.UI.Exceptions;
using Cinema.UI.Filters;
using Cinema.UI.Models;
using Cinema.UI.Models.SessionModels;
using Cinema.UI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cinema.UI.Controllers
{
    [ServiceFilter(typeof(AuthFilter))]
    public class SessionController : Controller
    {
        private readonly ICrudService _crudService;
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionController(ICrudService crudService, HttpClient client, IHttpContextAccessor httpContextAccessor)
        {
            _crudService = crudService;
            _client = client;
            _httpContextAccessor = httpContextAccessor;
            _client.BaseAddress = new Uri("https://localhost:44324/");
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                var sessions = await _crudService.GetAllPaginated<SessionListItemGetResponse>("sessions", page);

                if (page > sessions.TotalPages)
                {
                    return RedirectToAction("Index", new { page = sessions.TotalPages });
                }
                return View(sessions);
            }
            catch (HttpException e)
            {
                if (e.Status == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<IActionResult> Create()
        {
            try
            {
                ViewBag.Movies = await GetMovies();
                ViewBag.Halls = await GetHalls();
                ViewBag.Languages = await GetLanguages();

                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(SessionCreateRequest createRequest)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Movies = await GetMovies();
                ViewBag.Halls = await GetHalls();
                ViewBag.Languages = await GetLanguages();
                return View(createRequest);
            }

            try
            {
                var validLanguages = await GetLanguages();
                var movies = await GetMovies();

                var selectedMovie = movies.FirstOrDefault(m => m.Id == createRequest.MovieId);
                if (selectedMovie == null)
                {
                    ModelState.AddModelError("MovieId", "The selected movie does not exist.");
                    ViewBag.Movies = movies;
                    ViewBag.Halls = await GetHalls();
                    ViewBag.Languages = validLanguages;
                    return View(createRequest);
                }

                if (!await IsLanguageValidForMovie(createRequest.MovieId, createRequest.LanguageId))
                {
                    ModelState.AddModelError("LanguageId", "The selected language is not available for the selected movie.");
                    ViewBag.Movies = movies;
                    ViewBag.Halls = await GetHalls();
                    ViewBag.Languages = validLanguages;
                    return View(createRequest);
                }

                await _crudService.Create<SessionCreateRequest>(createRequest, "sessions");
                return RedirectToAction("Index");
            }
            catch (ModelException e)
            {
                foreach (var item in e.Error.Errors)
                {
                    ModelState.AddModelError(item.Key, item.Message);
                }

                ViewBag.Movies = await GetMovies();
                ViewBag.Halls = await GetHalls();
                ViewBag.Languages = await GetLanguages();
                return View(createRequest);
            }
            catch (HttpException ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Movies = await GetMovies();
                ViewBag.Halls = await GetHalls();
                ViewBag.Languages = await GetLanguages();
                return View(createRequest);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", $"Unexpected Error: {e.Message}");
                ViewBag.Movies = await GetMovies();
                ViewBag.Halls = await GetHalls();
                ViewBag.Languages = await GetLanguages();
                return View(createRequest);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var sessionDto = await _crudService.Get<SessionGetResponse>($"sessions/{id}");
                var editRequest = new SessionEditRequest
                {
                    MovieId = sessionDto.MovieId,
                    HallId = sessionDto.HallId,
                    LanguageId = sessionDto.LanguageId,
                    ShowDateTime = sessionDto.ShowDateTime,
                    Price = sessionDto.Price,
                    Duration = sessionDto.Duration
                };

                ViewBag.Movies = await GetMovies();
                ViewBag.Halls = await GetHalls();
                ViewBag.Languages = await GetLanguages();

                return View(editRequest);
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, SessionEditRequest editRequest)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Movies = await GetMovies();
                ViewBag.Halls = await GetHalls();
                ViewBag.Languages = await GetLanguages();
                return View(editRequest);
            }

            try
            {
                var existingSession = await _crudService.Get<SessionGetResponse>($"sessions/{id}");
                if (existingSession == null)
                {
                    return NotFound();
                }

                if (!editRequest.LanguageId.HasValue || !await IsLanguageValidForMovie(editRequest.MovieId.Value, editRequest.LanguageId.Value))
                {
                    ModelState.AddModelError("LanguageId", "The selected language is not available for the selected movie.");
                    ViewBag.Movies = await GetMovies();
                    ViewBag.Halls = await GetHalls();
                    ViewBag.Languages = await GetLanguages();
                    return View(editRequest);
                }

                await _crudService.Update<SessionEditRequest>(editRequest, $"sessions/{id}");
                return RedirectToAction("Index");
            }
            catch (ModelException e)
            {
                foreach (var item in e.Error.Errors)
                {
                    ModelState.AddModelError(item.Key, item.Message);
                }

                ViewBag.Movies = await GetMovies();
                ViewBag.Halls = await GetHalls();
                ViewBag.Languages = await GetLanguages();
                return View(editRequest);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", $"Error: {e.Message}");
                ViewBag.Movies = await GetMovies();
                ViewBag.Halls = await GetHalls();
                ViewBag.Languages = await GetLanguages();
                return View(editRequest);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _crudService.Delete($"sessions/{id}");
                return RedirectToAction("Index");
            }
            catch (HttpException e)
            {
                return StatusCode((int)e.Status);
            }
        }

        private async Task<List<Movie>> GetMovies()
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);
            using (var response = await _client.GetAsync("api/admin/Movies/all"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<Movie>>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
            }
            return new List<Movie>();
        }

        private async Task<List<Hall>> GetHalls()
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);
            using (var response = await _client.GetAsync("api/admin/Halls/all"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<Hall>>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
            }
            return new List<Hall>();
        }

        private async Task<List<Language>> GetLanguages()
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);
            using (var response = await _client.GetAsync("api/admin/Languages/all"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<Language>>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
            }
            return new List<Language>();
        }

        [HttpGet]
        public async Task<IActionResult> ExportToExcel()
        {
            try
            {
                var fileContent = await _crudService.SessionExcelExportAsync();
                return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Sessions.xlsx");
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

        private async Task<bool> IsLanguageValidForMovie(int movieId, int languageId)
        {
            var response = await _client.GetAsync($"api/admin/Movies/{movieId}/languages");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var languages = JsonSerializer.Deserialize<List<Language>>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return languages.Any(l => l.Id == languageId);
            }
            return false;
        }
    }
}