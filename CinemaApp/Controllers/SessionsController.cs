using Cinema.Core.Entites;
using Cinema.Service.Dtos.SessionDtos;
using Cinema.Service.Dtos;
using Cinema.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Cinema.Service.Implementations;
using Cinema.Service.Exceptions;
using Microsoft.AspNetCore.OutputCaching;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;


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
        [OutputCache(Duration = 600, VaryByQueryKeys = new[] { "id" })]
        public ActionResult<AdminSessionGetDto> GetById(int id)
        {
            var result = _sessionService.GetById(id);
            return Ok(result);
        }

        [HttpGet("byHall/{hallId}")]
        [OutputCache(Duration = 300, VaryByQueryKeys = new[] { "hallId" })]
        public async Task<IActionResult> GetSessionsByHall(int hallId)
        {
            var sessions = await _sessionService.GetSessionsByHall(hallId);
            return Ok(sessions);
        }

        [HttpGet("all")]
        [OutputCache(Duration = 3600)]
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

        [HttpGet("count/monthly")]
        [OutputCache(Duration = 600)]
        public async Task<IActionResult> GetSessionCountMonthly()
        {
            var count = await _sessionService.GetSessionCountLastMonthAsync();
            return Ok(new { count });
        }

        [HttpGet("{movieId}/sessions")]
        [OutputCache(Duration = 600, VaryByQueryKeys = new[] { "movieId", "date", "branchId", "languageId" })]
        public ActionResult<List<UserSessionDetailsDto>> GetSessionsByMovieIdAndDate(int movieId, DateTime? date = null, [FromQuery] int? branchId = null, [FromQuery] int? languageId = null)
        {
            var queryDate = date ?? DateTime.Now;
            var sessions = _sessionService.GetSessionsByMovieAndDateAsync(movieId, queryDate, branchId, languageId);
            return Ok(sessions);
        }

        [HttpGet("seats/{sessionId}")]
        [OutputCache(Duration = 300, VaryByQueryKeys = new[] { "sessionId" })]
        public async Task<IActionResult> GetSeatsForSession(int sessionId)
        {
            try
            {
                var seats = _sessionService.GetSeatsForSession(sessionId);
                return Ok(seats);
            }
            catch (RestException ex)
            {
                return StatusCode(ex.Code, ex.Message);
            }
        }
    }
}
