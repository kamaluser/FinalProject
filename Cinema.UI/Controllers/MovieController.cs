using Cinema.Core.Entites;
using Cinema.UI.Exceptions;
using Cinema.UI.Filters;
using Cinema.UI.Models.LanguageModels;
using Cinema.UI.Models.MovieModels;
using Cinema.UI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Text.Json;

namespace Cinema.UI.Controllers
{
    [ServiceFilter(typeof(AuthFilter))]
    public class MovieController : Controller
    {
        private readonly ICrudService _crudService;
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MovieController(ICrudService crudService, HttpClient client, IHttpContextAccessor httpContextAccessor)
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
                var movies = await _crudService.GetAllPaginated<MovieListItemGetResponse>("movies", page);
                if (page > movies.TotalPages)
                {
                    return RedirectToAction("Index", new { page = movies.TotalPages });
                }


                return View(movies);
            }
            catch (HttpException e)
            {
                if (e.Status == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("login", "account");
                }
                else
                {
                    return RedirectToAction("error", "home");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("error", "home");
            }
        }

        public async Task<IActionResult> Create()
        {
            try
            {
                ViewBag.Languages = await getLanguages();

                if (ViewBag.Languages == null) return RedirectToAction("error", "home");

                return View();
            }
            catch (Exception e)
            {
                return RedirectToAction("error", "home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(MovieCreateRequest createRequest)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Languages = await getLanguages();

                if (ViewBag.Languages == null) return RedirectToAction("error", "home");

                return View();
            }

            try
            {
                await _crudService.CreateFromForm<MovieCreateRequest>(createRequest, "movies");
                return RedirectToAction("index");
            }
            catch (ModelException e)
            {
                foreach (var item in e.Error.Errors)
                {
                    ModelState.AddModelError(item.Key, item.Message);
                }

                ViewBag.Languages = await getLanguages();

                if (ViewBag.Languages == null) return RedirectToAction("error", "home");


                return View();
            }
            catch (HttpException ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Languages = await getLanguages();

                if (ViewBag.Languages == null) return RedirectToAction("error", "home");

                return View();
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", $"Unexpected Error: {e.Message}");
                ViewBag.Languages = await getLanguages();

                if (ViewBag.Languages == null) return RedirectToAction("error", "home");

                return View();
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var movieDto = await _crudService.Get<MovieGetResponse>("movies/" + id);
                var editRequest = new MovieEditRequest
                {
                    Title = movieDto.Title,
                    Description = movieDto.Description,
                    ReleaseDate = movieDto.ReleaseDate,
                    TrailerLink = movieDto.TrailerLink,
                    AgeLimit = movieDto.AgeLimit,
                    PhotoUrl = movieDto.Photo,
                    LanguageIds = movieDto.LanguageIds
                };

                ViewBag.Languages = await getLanguages();

                if (ViewBag.Languages == null) return RedirectToAction("error", "home");

                return View(editRequest);
            }
            catch (Exception e)
            {
                return RedirectToAction("error", "home");
            }
        }


        [HttpPost]
        public async Task<IActionResult> Edit(int id, MovieEditRequest editRequest)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Languages = await getLanguages();

                if (ViewBag.Languages == null) return RedirectToAction("error", "home");

                return View(editRequest);
            }

            try
            {
                await _crudService.UpdateFromForm<MovieEditRequest>(editRequest, $"movies/{id}");
                return RedirectToAction("index");
            }
            catch (ModelException e)
            {
                foreach (var item in e.Error.Errors)
                {
                    ModelState.AddModelError(item.Key, item.Message);
                }

                ViewBag.Languages = await getLanguages();

                if (ViewBag.Languages == null) return RedirectToAction("error", "home");

                return View(editRequest);
            }
            catch (HttpException ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Languages = await getLanguages();

                if (ViewBag.Languages == null) return RedirectToAction("error", "home");

                return View(editRequest);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", $"Unexpected Error: {e.Message}");
                ViewBag.Languages = await getLanguages();

                if (ViewBag.Languages == null) return RedirectToAction("error", "home");

                return View(editRequest);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _crudService.Delete("movies/" + id);
                return Ok();
            }
            catch (HttpException e)
            {
                return StatusCode((int)e.Status);
            }
        }

        private async Task<List<LanguageListItemGetResponse>> getLanguages()
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);
            try
            {
                using (var response = await _client.GetAsync("api/admin/Languages/all"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var data = JsonSerializer.Deserialize<List<Language>>(await response.Content.ReadAsStringAsync(), options);

                        var languages = data.Select(l => new LanguageListItemGetResponse
                        {
                            Id = l.Id,
                            Name = l.Name,
                            FlagPhoto = l.FlagPhoto
                        }).ToList();

                        return languages;
                    }
                    else
                    {
                        Console.WriteLine($"Response failed with status code: {response.StatusCode}");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HttpRequestException: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
            return new List<LanguageListItemGetResponse>();
        }

    }
}