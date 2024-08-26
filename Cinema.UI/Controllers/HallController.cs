using Cinema.UI.Exceptions;
using Cinema.UI.Models.BranchModels;
using Cinema.UI.Models.HallModels;
using Cinema.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Cinema.UI.Controllers
{
    public class HallController : Controller
    {
        private readonly ICrudService _crudService;
        private readonly HttpClient _client;

        public HallController(ICrudService crudService, HttpClient client)
        {
            _crudService = crudService;
            _client = client;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                var halls = await _crudService.GetAllPaginated<HallListItemGetResponse>("halls", page);
                if (page > halls.TotalPages)
                {
                    return RedirectToAction("Index", new { page = halls.TotalPages });
                }


                return View(halls);
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
            var branches = GetAllBranches().Result;
            ViewBag.Branches = branches;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(HallCreateRequest createRequest)
        {
            if (!ModelState.IsValid)
            {
                var branches = await GetAllBranches();
                ViewBag.Branches = branches;
                return View(createRequest);
            }

            try
            {
                await _crudService.Create<HallCreateRequest>(createRequest, "halls");
                return RedirectToAction("index");
            }
            catch (ModelException e)
            {
                foreach (var item in e.Error.Errors)
                {
                    ModelState.AddModelError(item.Key, item.Message);
                }
                var branches = await GetAllBranches();
                ViewBag.Branches = branches;
                return View(createRequest);
            }
            catch (HttpException ex)
            {
                ModelState.AddModelError("", ex.Message);
                var branches = await GetAllBranches();
                ViewBag.Branches = branches;
                return View(createRequest);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", $"Unexpected Error: {e.Message}");
                var branches = await GetAllBranches();
                ViewBag.Branches = branches;
                return View(createRequest);
            }
        }



        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _crudService.Delete("halls/" + id);
                return Ok();
            }
            catch (HttpException e)
            {
                return StatusCode((int)e.Status);
            }
        }



        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var hall = await _crudService.Get<HallGetResponse>("halls/" + id);
                var editRequest = new HallEditRequest
                {
                    Name = hall.Name,
                    SeatCount = hall.SeatCount,
                    BranchId = hall.BranchId
                };

                ViewBag.Branches = await GetAllBranches();

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
                await Console.Out.WriteLineAsync(e.Message);
                return RedirectToAction("error", "home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, HallEditRequest editRequest)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Branches = await GetAllBranches();
                return View(editRequest);
            }
            try
            {
                var editDto = new HallEditRequest
                {
                    Name = editRequest.Name,
                    SeatCount = editRequest.SeatCount,

                };
                await _crudService.Update<HallEditRequest>(editRequest, $"halls/{id}");
                return RedirectToAction("index");
            }
            catch (ModelException e)
            {
                foreach (var item in e.Error.Errors)
                {
                    ModelState.AddModelError(item.Key, item.Message);
                }

                ViewBag.Branches = await GetAllBranches();
                return View(editRequest);
            }
            catch (HttpException ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Branches = await GetAllBranches();
                return View(editRequest);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", $"Unexpected Error: {e.Message}");
                ViewBag.Branches = await GetAllBranches();
                return View(editRequest);
            }
        }


        private async Task<List<BranchListItemGetResponse>> GetAllBranches()
        {
            using (var response = await _client.GetAsync("https://localhost:44324/api/admin/Branches/all"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    var data = JsonSerializer.Deserialize<List<BranchListItemGetResponse>>(await response.Content.ReadAsStringAsync(), options);

                    return data;
                }
            }
            return null;
        }

    }
}
