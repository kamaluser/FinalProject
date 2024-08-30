using AutoMapper;
using Cinema.Core.Entites;
using Cinema.Data.Repositories.Implementations;
using Cinema.Data.Repositories.Interfaces;
using Cinema.Service.Dtos;
using Cinema.Service.Dtos.LanguageDtos;
using Cinema.Service.Dtos.MovieDtos;
using Cinema.Service.Dtos.SessionDtos;
using Cinema.Service.Exceptions;
using Cinema.Service.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema.Service.Implementations
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly ILanguageRepository _languageRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _accessor;
        private readonly string _baseUrl;

        public MovieService(IMovieRepository movieRepository, ILanguageRepository languageRepository, IWebHostEnvironment environment, IMapper mapper, IHttpContextAccessor accessor, ISessionRepository sessionRepository)
        {
            _movieRepository = movieRepository;
            _languageRepository = languageRepository;
            _environment = environment;
            _mapper = mapper;
            _accessor = accessor;

            var uriBuilder = new UriBuilder(_accessor.HttpContext.Request.Scheme, _accessor.HttpContext.Request.Host.Host, _accessor.HttpContext.Request.Host.Port ?? -1);
            if (uriBuilder.Uri.IsDefaultPort)
            {
                uriBuilder.Port = -1;
            }
            _baseUrl = uriBuilder.Uri.AbsoluteUri;
            _sessionRepository = sessionRepository;
        }

        public PaginatedList<UserMovieGetDto> GetFutureMoviesWithPagination(int page = 1, int size = 10)
        {
            var futureDate = DateTime.Now.Date;

            var query = _movieRepository.GetAll(
                x => x.ReleaseDate > futureDate && !x.IsDeleted,
                "MovieLanguages.Language"
            )
            .OrderBy(x => x.ReleaseDate);

            var paginatedMovies = PaginatedList<Movie>.Create(query, page, size);

            var movieDtos = _mapper.Map<List<UserMovieGetDto>>(paginatedMovies.Items);

            return new PaginatedList<UserMovieGetDto>(movieDtos, paginatedMovies.TotalPages, paginatedMovies.PageIndex, paginatedMovies.PageSize)
            {
                HasPrev = paginatedMovies.HasPrev,
                HasNext = paginatedMovies.HasNext
            };
        }

        public PaginatedList<UserMovieGetDto> GetMoviesForTodayWithPagination(int page = 1, int size = 8)
        {
            var today = DateTime.UtcNow.Date;

            var query = _movieRepository.GetAll(x => x.Sessions.Any(s => s.ShowDateTime.Date == today && !s.IsDeleted))
                                        .Include(x => x.Sessions)
                                        .ThenInclude(x => x.Language)
                                        .Include(s => s.MovieLanguages)
                                        .ThenInclude(ml => ml.Language);

            var paginatedMovies = PaginatedList<Movie>.Create(query, page, size);

            var movieDtos = _mapper.Map<List<UserMovieGetDto>>(paginatedMovies.Items);

            return new PaginatedList<UserMovieGetDto>(movieDtos, paginatedMovies.TotalPages, paginatedMovies.PageIndex, paginatedMovies.PageSize)
            {
                HasPrev = paginatedMovies.HasPrev,
                HasNext = paginatedMovies.HasNext
            };
        }

        public List<UserMovieGetDto> GetMoviesForToday(int limit = 8)
        {
            var today = DateTime.Today;

            var movies = _movieRepository.GetAll(x => x.Sessions.Any(s => s.ShowDateTime.Date == today && !s.IsDeleted))
                                          .Include(x => x.Sessions.Where(s => s.ShowDateTime.Date == today && !s.IsDeleted))
                                          .ThenInclude(s => s.Language)
                                          .Take(limit)
                                          .ToList();

            var result = movies.Select(movie => new UserMovieGetDto
            {
                MovieName = movie.Title,
                MoviePhoto = $"{_baseUrl}/uploads/movies/{movie.Photo}",
                Languages = movie.Sessions.Select(s => new LanguageGetDto
                {
                    LanguageName = s.Language.Name,
                    LanguagePhoto = $"{_baseUrl}/uploads/flags/{s.Language.FlagPhoto}"
                }).ToList(),
                AgeLimit = movie.AgeLimit
            }).ToList();

            return result;
        }

        public int Create(AdminMovieCreateDto createDto)
        {
            if (_movieRepository.Exists(x => x.Title.ToLower() == createDto.Title.ToLower() && !x.IsDeleted))
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Title", "A movie with the given title already exists.");
            }

            var movie = _mapper.Map<Movie>(createDto);

            var languages = _languageRepository.GetAll(x => createDto.LanguageIds.Contains(x.Id)).ToList();
            if (languages.Count != createDto.LanguageIds.Count)
            {
                throw new RestException(StatusCodes.Status404NotFound, "LanguageIds", "One or more languages not found.");
            }

            movie.MovieLanguages = languages.Select(l => new MovieLanguage { Movie = movie, Language = l }).ToList();

            if (createDto.Photo != null)
            {
                var photoPath = SavePhoto(createDto.Photo);
                movie.Photo = photoPath;
            }

            _movieRepository.Add(movie);
            _movieRepository.Save();

            return movie.Id;
        }

        public PaginatedList<AdminMovieGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {
            var query = _movieRepository.GetAll(
                x => (search == null || x.Title.Contains(search)) && !x.IsDeleted,
                "MovieLanguages.Language"
            );

            var paginated = PaginatedList<Movie>.Create(query, page, size);
            var dtos = _mapper.Map<List<AdminMovieGetDto>>(paginated.Items);
            return new PaginatedList<AdminMovieGetDto>(dtos, paginated.TotalPages, page, size);
        }

        public void Edit(int id, AdminMovieEditDto editDto)
        {
            var movie = _movieRepository.Get(x => x.Id == id && !x.IsDeleted, "MovieLanguages");

            if (movie == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, $"Movie with {id} ID not found.");
            }

            if (_movieRepository.Exists(x => x.Title.ToLower() == editDto.Title.ToLower() && x.Id != id && !x.IsDeleted))
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Title", "A movie with the given title already exists.");
            }

            var languages = _languageRepository.GetAll(x => editDto.LanguageIds.Contains(x.Id)).ToList();
            if (languages.Count != editDto.LanguageIds.Count)
            {
                throw new RestException(StatusCodes.Status404NotFound, "LanguageIds", "One or more languages not found.");
            }

            if (editDto.Photo != null)
            {
                var photoPath = SavePhoto(editDto.Photo);
                if (!string.IsNullOrEmpty(movie.Photo) && System.IO.File.Exists(Path.Combine(_environment.WebRootPath, "uploads", "movies", movie.Photo)))
                {
                    System.IO.File.Delete(Path.Combine(_environment.WebRootPath, "uploads", "movies", movie.Photo));
                }
                movie.Photo = photoPath;
            }

            _mapper.Map(editDto, movie);

            movie.MovieLanguages = new List<MovieLanguage>();

            foreach (var item in languages)
            {
                MovieLanguage ml = new MovieLanguage
                {
                    Movie = movie,
                    Language = item
                };
                movie.MovieLanguages.Add(ml);
            }
            movie.ModifiedAt = DateTime.Now;

            _movieRepository.Save();
        }

        public void Delete(int id)
        {
            var movie = _movieRepository.Get(x => x.Id == id && !x.IsDeleted);
            if (movie == null)
                throw new RestException(StatusCodes.Status404NotFound, $"Movie with {id} ID not found.");

            movie.IsDeleted = true;
            movie.ModifiedAt = DateTime.Now;
            _movieRepository.Save();
        }

        public AdminMovieGetDto GetById(int id)
        {
            var movie = _movieRepository.Get(x => x.Id == id && !x.IsDeleted, "MovieLanguages.Language");
            if (movie == null)
                throw new RestException(StatusCodes.Status404NotFound, $"Movie with {id} ID not found.");

            return _mapper.Map<AdminMovieGetDto>(movie);
        }

        public List<AdminMovieGetDto> GetAll(string? search = null)
        {
            var movies = _movieRepository.GetAll(
                x => (search == null || x.Title.Contains(search)) && !x.IsDeleted,
                "MovieLanguages.Language"
            ).ToList();

            return _mapper.Map<List<AdminMovieGetDto>>(movies);
        }

        private string SavePhoto(IFormFile photo)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
            var filePath = Path.Combine(_environment.WebRootPath, "uploads", "movies", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                photo.CopyTo(stream);
            }

            return fileName;
        }

        private void DeletePhoto(string photoFileName)
        {
            var filePath = Path.Combine(_environment.WebRootPath, "uploads", "movies", photoFileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public List<MovieLanguageDto> GetLanguagesByMovieId(int movieId)
        {
            var movie = _movieRepository.Get(x => x.Id == movieId && !x.IsDeleted, "MovieLanguages.Language");
            if (movie == null)
                throw new RestException(StatusCodes.Status404NotFound, $"Movie with {movieId} ID not found.");

            return movie.MovieLanguages
                        .Select(ml => new MovieLanguageDto
                        {
                            Id = ml.Language.Id,
                            Name = ml.Language.Name
                        }).ToList();
        }


        public List<UserMovieGetDto> GetMoviesByFiltersAsync(DateTime? date = null, int? branchId = null, int? languageId = null)
        {
            var selectedDate = date ?? DateTime.Now;

            var sessionsQuery = _sessionRepository.GetAll(s => s.ShowDateTime.Date == selectedDate.Date && !s.IsDeleted)
                .Include(s => s.Movie)
                .Include(s => s.Hall)
                .ThenInclude(h => h.Branch)
                .Include(s => s.Language)
                .ToList();

            if (branchId.HasValue)
            {
                sessionsQuery = sessionsQuery.Where(s => s.Hall.Branch.Id == branchId.Value).ToList();
            }

            if (languageId.HasValue)
            {
                sessionsQuery = sessionsQuery.Where(s => s.Language.Id == languageId.Value).ToList();
            }

            var movies = sessionsQuery
                .GroupBy(s => s.Movie.Id)
                .Select(g => new UserMovieGetDto
                {
                    MovieName = g.First().Movie.Title,
                    MoviePhoto = $"{_baseUrl}/uploads/movies/{g.First().Movie.Photo}",
                    AgeLimit = g.First().Movie.AgeLimit,
                    ReleaseDate = g.First().Movie.ReleaseDate,
                    Languages = g.Select(s => new LanguageGetDto
                    {
                        LanguageName = s.Language.Name,
                        LanguagePhoto = $"{_baseUrl}/uploads/flags/{s.Language.FlagPhoto}"
                    }).Distinct().ToList()
                })
                .ToList();

            return movies;
        }

    }
}
