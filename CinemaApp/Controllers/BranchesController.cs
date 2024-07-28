using Cinema.Service.Dtos;
using Cinema.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Cinema.Service.Dtos.BranchDtos;
using Cinema.Core.Entites;
using Microsoft.AspNetCore.Authorization;

namespace CinemaApp.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class BranchesController : Controller
    {
        private readonly IBranchService _branchService;

        public BranchesController(IBranchService branchService)
        {
            _branchService = branchService;
        }

        [HttpPost("")]
        public ActionResult<int> Create(AdminBranchCreateDto createDto)
        {
            var id = _branchService.Create(createDto);
            return StatusCode(201, new { id });
        }

        [HttpGet("")]
        public ActionResult<PaginatedList<AdminBranchGetDto>> GetAllByPagination(string? search = null, int page = 1, int size = 3)
        {
            return StatusCode(200, _branchService.GetAllByPage(search, page, size));
        }

        [HttpGet("all")]
        public ActionResult<List<Branch>> GetAll(string? search = null)
        {
            var branches = _branchService.GetAll(search);
            return Ok(branches);
        }

        [HttpGet("{id}")]
        public ActionResult<AdminBranchGetDto> GetById(int id)
        {
            var result = _branchService.GetById(id);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public ActionResult Edit(int id, AdminBranchEditDto editDto)
        {
            _branchService.Edit(id, editDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            _branchService.Delete(id);
            return NoContent();
        }
    }
}
