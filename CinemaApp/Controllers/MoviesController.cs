using Cinema.Core.Entites;
using Cinema.Service.Dtos.MovieDtos;
using Cinema.Service.Dtos;
using Cinema.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Cinema.Service.Dtos.SessionDtos;

namespace CinemaApp.Controllers
{
    [ApiController]
    public class MoviesController : Controller
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPost("api/admin/[controller]")]
        public ActionResult<int> Create(AdminMovieCreateDto createDto)
        {
            var id = _movieService.Create(createDto);
            return StatusCode(201, new { id });
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpGet("api/admin/[controller]")]
        public ActionResult<PaginatedList<AdminMovieGetDto>> GetAllByPagination(string? search = null, int page = 1, int size = 3)
        {
            return StatusCode(200, _movieService.GetAllByPage(search, page, size));
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpGet("api/admin/[controller]/all")]
        public ActionResult<List<Movie>> GetAll(string? search = null)
        {
            var Moviees = _movieService.GetAll(search);
            return Ok(Moviees);
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpGet("api/admin/[controller]/{id}")]
        public ActionResult<AdminMovieGetDto> GetById(int id)
        {
            var result = _movieService.GetById(id);
            return Ok(result);
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPut("api/admin/[controller]/{id}")]
        public ActionResult Edit(int id, AdminMovieEditDto editDto)
        {
            _movieService.Edit(id, editDto);
            return NoContent();
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpDelete("api/admin/[controller]/{id}")]
        public ActionResult Delete(int id)
        {
            _movieService.Delete(id);
            return NoContent();
        }
        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("api/[controller]/{id}/languages")]
        public ActionResult<List<MovieLanguageDto>> GetLanguagesByMovieId(int id)
        {
            var languages = _movieService.GetLanguagesByMovieId(id);
            return Ok(languages);
        }
        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("api/[controller]/todaysMovies")]
        public ActionResult<List<UserMovieGetDto>> GetMoviesForToday(int limit = 8)
        {
            var movies = _movieService.GetMoviesForToday(limit);
            return Ok(movies);
        }

        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("api/[controller]/todaysMovies/paginated")]
        public ActionResult<PaginatedList<UserMovieGetDto>> GetMoviesForTodayWithPagination(int page = 1, int size = 6)
        {
            var movies = _movieService.GetMoviesForTodayWithPagination(page, size);
            return Ok(movies);
        }

        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("api/[controller]/future-movies")]
        public ActionResult<PaginatedList<AdminMovieGetDto>> GetFutureMovies([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var result = _movieService.GetFutureMoviesWithPagination(page, size);
            return Ok(result);
        }
    }
}
