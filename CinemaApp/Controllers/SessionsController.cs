using Cinema.Core.Entites;
using Cinema.Service.Dtos.SessionDtos;
using Cinema.Service.Dtos;
using Cinema.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Cinema.Service.Implementations;

namespace CinemaApp.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class SessionsController : Controller
    {
        private readonly ISessionService _sessionService;

        public SessionsController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [HttpPost("")]
        public ActionResult<int> Create(AdminSessionCreateDto createDto)
        {
            var id = _sessionService.Create(createDto);
            return StatusCode(201, new { id });
        }

        [HttpGet("")]
        public ActionResult<PaginatedList<AdminSessionGetDto>> GetAllByPagination(string? search = null, int page = 1, int size = 3)
        {
            return StatusCode(200, _sessionService.GetAllByPage(search, page, size));
        }

        [HttpGet("{id}")]
        public ActionResult<AdminSessionGetDto> GetById(int id)
        {
            var result = _sessionService.GetById(id);
            return Ok(result);
        }

        [HttpGet("byHall/{hallId}")]
        public async Task<IActionResult> GetSessionsByHall(int hallId)
        {
            var sessions = await _sessionService.GetSessionsByHall(hallId);
            return Ok(sessions);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllSessions()
        {
            var sessions = _sessionService.GetAll();
            return Ok(sessions);
        }


        [HttpPut("{id}")]
        public ActionResult Edit(int id, AdminSessionEditDto editDto)
        {
            _sessionService.Edit(id, editDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            _sessionService.Delete(id);
            return NoContent();
        }
    }
}