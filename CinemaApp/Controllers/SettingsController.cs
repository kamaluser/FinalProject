using Cinema.Service.Dtos.SettingDtos;
using Cinema.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CinemaApp.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingService _settingService;

        public SettingsController(ISettingService settingService)
        {
            _settingService = settingService;
        }

        [HttpGet]
        public ActionResult<AdminSettingGetDto> Get()
        {
            var setting = _settingService.Get();
            return Ok(setting);
        }

        [HttpPut]
        public IActionResult Edit([FromForm] AdminSettingEditDto editDto)
        {
            _settingService.Edit(editDto);
            return NoContent();
        }

        [HttpPost]
        public IActionResult Create([FromForm] AdminSettingCreateDto createDto)
        {
            _settingService.Create(createDto);
            return CreatedAtAction(nameof(Get), new { id = createDto.PhoneNumber }, createDto);
        }
    }
}
