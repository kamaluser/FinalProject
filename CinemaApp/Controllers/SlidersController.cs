using Cinema.Service.Dtos;
using Cinema.Service.Dtos.SliderDtos;
using Cinema.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaApp.Controllers
{
    [ApiExplorerSettings(GroupName = "admin_v1")]
    [Route("api/admin/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class SlidersController : Controller
    {
        private readonly ISliderService _sliderService;

        public SlidersController(ISliderService sliderService)
        {
            _sliderService = sliderService;
        }

        [HttpPost("")]
        public ActionResult<int> Create(AdminSliderCreateDto createDto)
        {
            var id = _sliderService.Create(createDto);
            return StatusCode(201, new { id });
        }

        [HttpGet("")]
        public ActionResult<PaginatedList<AdminSliderGetDto>> GetAll(int page = 1, int size = 3)
        {
            if (page < 1) page = 1;
            if (size < 1) size = 3;

            var paginatedList = _sliderService.GetAllByPage(page, size);

            if (!paginatedList.Items.Any() && page > 1)
            {
                return BadRequest(new { message = "No sliders found for this page." });
            }

            return StatusCode(200, paginatedList);
        }

        [HttpGet("{id}")]
        public ActionResult<AdminSliderGetDto> GetById(int id)
        {
            var result = _sliderService.GetById(id);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public ActionResult Edit(int id, AdminSliderEditDto editDto)
        {
            _sliderService.Edit(id, editDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            _sliderService.Delete(id);
            return NoContent();
        }

    }
}
