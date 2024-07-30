using Cinema.UI.Exceptions;
using Cinema.UI.Models;
using Cinema.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

public class MovieController : Controller
{
    private HttpClient _client;
    private readonly ICrudService _crudService;

    public MovieController(ICrudService crudService, HttpClient client)
    {
        _crudService = crudService;
        _client = new HttpClient();
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        try
        {
            return View(await _crudService.GetAllPaginated<MovieListItemGetResponse>("movies", page));
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
            await _crudService.Create<MovieCreateRequest>(createRequest, "movies");
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
                LanguageIds = movieDto.Languages?.Select(l => l.Id).ToList()
            };

            ViewBag.Languages = await getLanguages();

            if (ViewBag.Languages == null) return RedirectToAction("error", "home");


            return View(editRequest);
        }
        catch (ModelException e)
        {
            foreach (var item in e.Error.Errors)
            {
                ModelState.AddModelError(item.Key, item.Message);
            }

            return View();
        }
        catch (HttpException e)
        {
            if (e.Status == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
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
            await _crudService.Update<MovieEditRequest>(editRequest, $"movies/{id}");
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

    private async Task<List<MovieListItemGetResponse>> getLanguages()
    {
        using (var response = await _client.GetAsync("http://localhost:5194/api/admin/Languages/all"))
        {
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                var data = JsonSerializer.Deserialize<List<MovieListItemGetResponse>>(await response.Content.ReadAsStringAsync(), options);

                return data;
            }
        }
        return null;
    }
}
