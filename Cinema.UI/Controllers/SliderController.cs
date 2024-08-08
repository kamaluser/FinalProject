using Cinema.UI.Exceptions;
using Cinema.UI.Filters;
using Cinema.UI.Models.SliderModels;
using Cinema.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.UI.Controllers
{
    [ServiceFilter(typeof(AuthFilter))]
    public class SliderController : Controller
    {
        private readonly ICrudService _crudService;

        public SliderController(ICrudService crudService)
        {
            _crudService = crudService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                return View(await _crudService.GetAllPaginated<SliderListItemGetResponse>("sliders", page));
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
        public async Task<IActionResult> Create(SliderCreateRequest createRequest)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                await _crudService.CreateFromForm<SliderCreateRequest>(createRequest, "sliders");
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
                var slider = await _crudService.Get<SliderGetResponse>("sliders/" + id);
                var editRequest = new SliderEditRequest
                {
                    Order = slider.Order,
                    ImageUrl = slider.Image,
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
        public async Task<IActionResult> Edit(int id, SliderEditRequest editRequest)
        {
            if (!ModelState.IsValid)
            {
                return View(editRequest);
            }

            try
            {
                await _crudService.UpdateFromForm<SliderEditRequest>(editRequest, $"sliders/{id}");
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
                await _crudService.Delete("sliders/" + id);
                return Ok();
            }
            catch (HttpException e)
            {
                return StatusCode((int)e.Status);
            }
        }
    }
}
