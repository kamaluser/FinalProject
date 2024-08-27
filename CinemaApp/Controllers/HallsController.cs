using Cinema.Core.Entites;
using Cinema.Service.Dtos.HallDtos;
using Cinema.Service.Implementations;
using Cinema.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaApp.Controllers
{
    [ApiExplorerSettings(GroupName = "admin_v1")]
    [Route("api/admin/[controller]")]
    [ApiController]
    public class HallsController : Controller
    {
        private readonly IHallService _hallService;

        public HallsController(IHallService hallService)
        {
            _hallService = hallService;
        }
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPost("")]
        public ActionResult Create(AdminHallCreateDto createDto)
        {
            return StatusCode(201, new { id = _hallService.Create(createDto) });
        }
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpGet("")]
        public ActionResult<List<AdminHallGetDto>> GetAllByPage(string? search = null, int page = 1, int size = 6)
        {
            return Ok(_hallService.GetAllByPage(search, page, size));
        }
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpGet("all")]
        public ActionResult<List<Branch>> GetAll(string? search = null)
        {
            var branches = _hallService.GetAll(search);
            return Ok(branches);
        }
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpGet("{id}")]
        public ActionResult<AdminHallGetDto> GetById(int id)
        {
            return Ok(_hallService.GetById(id));
        }
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPut("{id}")]
        public ActionResult Edit(int id, AdminHallEditDto editDto)
        {
            _hallService.Edit(id, editDto);
            return NoContent();
        }
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            _hallService.Delete(id);
            return NoContent();
        }
    }
}
