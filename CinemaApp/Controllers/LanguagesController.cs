using Cinema.Service.Dtos.SliderDtos;
using Cinema.Service.Dtos;
using Cinema.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Cinema.Service.Dtos.LanguageDtos;
using Cinema.Core.Entites;
using Cinema.Service.Dtos.HallDtos;
using Cinema.Service.Implementations;
using Microsoft.AspNetCore.Authorization;

namespace CinemaApp.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class LanguagesController : Controller
    {
        private readonly ILanguageService _languageService;

        public LanguagesController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        [HttpPost("")]
        public ActionResult<int> Create(AdminLanguageCreateDto createDto)
        {
            var id = _languageService.Create(createDto);
            return StatusCode(201, new { id });
        }

        [HttpGet("")]
        public ActionResult<List<AdminLanguageGetDto>> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {
            return Ok(_languageService.GetAllByPage(search, page, size));
        }

        [HttpGet("all")]
        public ActionResult<List<Language>> GetAll(string? search = null)
        {
            var branches = _languageService.GetAll(search);
            return Ok(branches);
        }

        [HttpGet("{id}")]
        public ActionResult<AdminLanguageGetDto> GetById(int id)
        {
            var result = _languageService.GetById(id);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public ActionResult Edit(int id, AdminLanguageEditDto editDto)
        {
            _languageService.Edit(id, editDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            _languageService.Delete(id);
            return NoContent();
        }

    }
}
