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
    [ApiController]
    public class SessionsController : Controller
    {
        private readonly ISessionService _sessionService;

        public SessionsController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPost("api/admin/sessions")]
        public ActionResult<int> Create(AdminSessionCreateDto createDto)
        {
            var id = _sessionService.Create(createDto);
            return StatusCode(201, new { id });
        }


        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpGet("api/admin/sessions")]
        public ActionResult<PaginatedList<AdminSessionGetDto>> GetAllByPagination(string? search = null, int page = 1, int size = 3)
        {
            return StatusCode(200, _sessionService.GetAllByPage(search, page, size));
        }


        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpGet("api/admin/sessions/{id}")]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
        public ActionResult<AdminSessionGetDto> GetById(int id)
        {
            var result = _sessionService.GetById(id);
            return Ok(result);
        }


        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpGet("api/admin/sessions/byHall/{hallId}")]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]

        public async Task<IActionResult> GetSessionsByHall(int hallId)
        {
            var sessions = await _sessionService.GetSessionsByHall(hallId);
            return Ok(sessions);
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpGet("api/admin/sessions/all")]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]

        public async Task<IActionResult> GetAllSessions()
        {
            var sessions = _sessionService.GetAll();
            return Ok(sessions);
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPut("api/admin/sessions/{id}")]
        public ActionResult Edit(int id, AdminSessionEditDto editDto)
        {
            _sessionService.Edit(id, editDto);
            return NoContent();
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpDelete("api/admin/sessions/{id}")]
        public ActionResult Delete(int id)
        {
            _sessionService.Delete(id);
            return NoContent();
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpGet("api/admin/sessions/count/monthly")]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
        public async Task<IActionResult> GetSessionCountMonthly()
        {
            var count = await _sessionService.GetSessionCountLastMonthAsync();
            return Ok(new { count });
        }

        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("api/sessions/{movieId}/ByMovie")]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
        public ActionResult<List<UserSessionDetailsDto>> GetSessionsByMovieIdAndDate(int movieId, DateTime? date = null, [FromQuery] int? branchId = null, [FromQuery] int? languageId = null)
        {
            var queryDate = date ?? DateTime.Now;
            var sessions = _sessionService.GetSessionsByMovieAndDateAsync(movieId, queryDate, branchId, languageId);
            return Ok(sessions);
        }

        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("api/sessions/{sessionId}/seats")]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
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

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpGet("api/admin/sessions/count/languages/monthly")]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
        public async Task<IActionResult> GetSessionCountByLanguageThisMonth()
        {
            var result = await _sessionService.GetSessionCountByLanguageThisMonthAsync();
            return Ok(result);
        }


        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("api/sessions/filtered-sessions")]
        public ActionResult<List<UserSessionDetailsDto>> GetSessionsByFilter(DateTime? date = null, [FromQuery] int? branchId = null, [FromQuery] int? languageId = null)
        {
            var queryDate = date ?? DateTime.Now;

            var sessions = _sessionService.GetSessionsByFiltersAsync(queryDate, branchId, languageId);

            return Ok(sessions);
        }

    }
}
