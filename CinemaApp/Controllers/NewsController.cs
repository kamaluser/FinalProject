using Cinema.Core.Entites;
using Cinema.Service.Dtos;
using Cinema.Service.Dtos.LanguageDtos;
using Cinema.Service.Dtos.NewsDtos;
using Cinema.Service.Dtos.SliderDtos;
using Cinema.Service.Implementations;
using Cinema.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaApp.Controllers
{
    [ApiController]
    public class NewsController:Controller
    {

        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPost("api/admin/news")]
        public ActionResult<int> Create(AdminNewsCreateDto createDto)
        {
            var id = _newsService.Create(createDto);
            return StatusCode(201, new { id });
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpGet("api/admin/news")]
        public ActionResult<List<AdminNewsGetDto>> GetAllByPage(string? search = null, int page = 1, int size = 3)
        {
            return Ok(_newsService.GetAllByPage(search, page, size));
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpGet("api/admin/news/{id}")]
        public ActionResult<AdminNewsGetDto> GetById(int id)
        {
            var result = _newsService.GetById(id);
            return Ok(result);
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPut("api/admin/news/{id}")]
        public ActionResult Edit(int id, AdminNewsEditDto editDto)
        {
            _newsService.Edit(id, editDto);
            return NoContent();
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpDelete("api/admin/news/{id}")]
        public ActionResult Delete(int id)
        {
            _newsService.Delete(id);
            return NoContent();
        }


        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("api/news/allbypage")]
        public ActionResult<PaginatedList<UserNewsGetDto>> GetAllByPageForUser(int page = 1, int size = 3)
        {
            if (page < 1) page = 1;
            if (size < 1) size = 3;

            var paginatedList = _newsService.GetAllByPageUser(page, size);

            return StatusCode(200, paginatedList);
        }
    }
}
