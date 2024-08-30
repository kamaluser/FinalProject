using Cinema.Service.Dtos.MovieDtos;
using Cinema.Service.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cinema.Service.Dtos.SessionDtos;

namespace Cinema.Service.Interfaces
{
    public interface IMovieService
    {
        int Create(AdminMovieCreateDto createDto);
        PaginatedList<AdminMovieGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10);
        void Edit(int id, AdminMovieEditDto editDto);
        void Delete(int id);
        AdminMovieGetDto GetById(int id);
        List<AdminMovieGetDto> GetAll(string? search = null);
        List<MovieLanguageDto> GetLanguagesByMovieId(int movieId);
        List<UserMovieGetDto> GetMoviesForToday(int limit);
        PaginatedList<UserMovieGetDto> GetMoviesForTodayWithPagination(int page = 1, int size = 8);
        PaginatedList<UserMovieGetDto> GetFutureMoviesWithPagination(int page = 1, int size = 10);
        List<UserMovieGetDto> GetMoviesByFiltersAsync(DateTime? date = null, int? branchId = null, int? languageId = null);
    }
}