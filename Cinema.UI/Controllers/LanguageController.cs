using Cinema.UI.Exceptions;
using Cinema.UI.Filters;
using Cinema.UI.Models.LanguageModels;
using Cinema.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.UI.Controllers
{
    [ServiceFilter(typeof(AuthFilter))]
    public class LanguageController : Controller
    {
        private readonly ICrudService _crudService;

        public LanguageController(ICrudService crudService)
        {
            _crudService = crudService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                var languages = await _crudService.GetAllPaginated<LanguageListItemGetResponse>("languages", page);
                if (page > languages.TotalPages)
                {
                    return RedirectToAction("Index", new { page = languages.TotalPages });
                }

                return View(languages);
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
        public async Task<IActionResult> Create(LanguageCreateRequest createRequest)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                await _crudService.CreateFromForm<LanguageCreateRequest>(createRequest, "languages");
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
                var languageDto = await _crudService.Get<LanguageGetResponse>("languages/" + id);
                var editRequest = new LanguageEditRequest
                {
                    Name = languageDto.Name,
                    FlagPhotoUrl = languageDto.FlagPhoto
                };
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
        public async Task<IActionResult> Edit(int id, LanguageEditRequest editRequest)
        {
            if (!ModelState.IsValid)
            {
                return View(editRequest);
            }

            try
            {
                await _crudService.UpdateFromForm<LanguageEditRequest>(editRequest, $"languages/{id}");
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
                await _crudService.Delete("languages/" + id);
                return Ok();
            }
            catch (HttpException e)
            {
                return StatusCode((int)e.Status);
            }
        }
    }
}
