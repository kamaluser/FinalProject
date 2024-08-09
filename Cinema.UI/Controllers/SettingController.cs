using Cinema.UI.Exceptions;
using Cinema.UI.Models.SettingModels;
using Cinema.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Cinema.UI.Controllers
{
    public class SettingController : Controller
    {
        private readonly ICrudService _crudService;

        public SettingController(ICrudService crudService)
        {
            _crudService = crudService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var setting = await _crudService.Get<SettingGetResponse>("settings");
                return View(setting);
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

        public async Task<IActionResult> Edit()
        {
            try
            {
                var setting = await _crudService.Get<SettingGetResponse>("settings");
                var editRequest = new SettingEditRequest
                {
                    PhoneNumber = setting.PhoneNumber,
                    FacebookUrl = setting.FacebookUrl,
                    YoutubeUrl = setting.YoutubeUrl,
                    InstagramUrl = setting.InstagramUrl,
                    TelegramUrl = setting.TelegramUrl,
                    ContactAddress = setting.ContactAddress,
                    ContactEmailAddress = setting.ContactEmailAddress,
                    ContactWorkHours = setting.ContactWorkHours,
                    ContactMarketingDepartment = setting.ContactMarketingDepartment,
                    ContactMap = setting.ContactMap,
                    AboutTitle = setting.AboutTitle,
                    AboutDesc = setting.AboutDesc
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
        public async Task<IActionResult> Edit(SettingEditRequest editRequest)
        {
            if (!ModelState.IsValid)
            {
                return View(editRequest);
            }

            try
            {
                await _crudService.UpdateFromForm<SettingEditRequest>(editRequest, "settings");
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
    }
}
