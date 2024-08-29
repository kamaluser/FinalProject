using Cinema.Service.Dtos;
using Cinema.Service.Dtos.SliderDtos;
using Cinema.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaApp.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class SlidersController : Controller
    {
        private readonly ISliderService _sliderService;

        public SlidersController(ISliderService sliderService)
        {
            _sliderService = sliderService;
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPost("")]
        public ActionResult<int> Create(AdminSliderCreateDto createDto)
        {
            var id = _sliderService.Create(createDto);
            return StatusCode(201, new { id });
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpGet("")]
        public ActionResult<PaginatedList<AdminSliderGetDto>> GetAll(int page = 1, int size = 3)
        {
            if (page < 1) page = 1;
            if (size < 1) size = 3;

            var paginatedList = _sliderService.GetAllByPage(page, size);

            return StatusCode(200, paginatedList);
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpGet("{id}")]
        public ActionResult<AdminSliderGetDto> GetById(int id)
        {
            var result = _sliderService.GetById(id);
            return Ok(result);
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPut("{id}")]
        public ActionResult Edit(int id, AdminSliderEditDto editDto)
        {
            _sliderService.Edit(id, editDto);
            return NoContent();
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            _sliderService.Delete(id);
            return NoContent();
        }

        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("all")]
        public ActionResult<List<UserSliderGetDto>> GetAllForUser()
        {
            return StatusCode(200, _sliderService.GetAllUser());
        }
    }
}