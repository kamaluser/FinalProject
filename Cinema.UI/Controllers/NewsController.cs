using Cinema.UI.Exceptions;
using Cinema.UI.Models.NewsModels;
using Cinema.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.UI.Controllers
{
    public class NewsController : Controller
    {
        private readonly ICrudService _crudService;

        public NewsController(ICrudService crudService)
        {
            _crudService = crudService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                var news = await _crudService.GetAllPaginated<NewsListItemGetResponse>("news", page);
                if (page > news.TotalPages)
                {
                    return RedirectToAction("Index", new { page = news.TotalPages });
                }


                return View(news);
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

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(NewsCreateRequest createRequest)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                await _crudService.CreateFromForm<NewsCreateRequest>(createRequest, "news");
                return RedirectToAction("index");
            }
            catch (ModelException e)
            {
                foreach (var item in e.Error.Errors)
                {
                    ModelState.AddModelError(item.Key, item.Message);
                }
                return View();
            }
            catch (HttpException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", $"Unexpected Error: {e.Message}");
                return View();
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var news = await _crudService.Get<NewsGetResponse>("news/" + id);
                var editRequest = new NewsEditRequest
                {
                    Title = news.Title,
                    Description = news.Description,
                    ImageUrl = news.Image
                };
                return View(editRequest);
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
        public async Task<IActionResult> Edit(int id, NewsEditRequest editRequest)
        {
            if (!ModelState.IsValid)
            {
                return View(editRequest);
            }

            try
            {
                await _crudService.UpdateFromForm<NewsEditRequest>(editRequest, $"news/{id}");
                return RedirectToAction("index");
            }
            catch (ModelException e)
            {
                foreach (var item in e.Error.Errors)
                {
                    ModelState.AddModelError(item.Key, item.Message);
                }

                return View(editRequest);
            }
            catch (HttpException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(editRequest);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", $"Unexpected Error: {e.Message}");
                return View(editRequest);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _crudService.Delete("news/" + id);
                return Ok();
            }
            catch (HttpException e)
            {
                return StatusCode((int)e.Status);
            }
        }
    }
}
