using AutoMapper;
using Cinema.Core.Entites;
using Cinema.Data.Repositories.Interfaces;
using Cinema.Service.Dtos;
using Cinema.Service.Dtos.MovieDtos;
using Cinema.Service.Exceptions;
using Cinema.Service.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
        private readonly IWebHostEnvironment _environment;
        private readonly IMapper _mapper;

        public MovieService(IMovieRepository movieRepository, ILanguageRepository languageRepository, IWebHostEnvironment environment, IMapper mapper)
        {
            _movieRepository = movieRepository;
            _languageRepository = languageRepository;
            _environment = environment;
            _mapper = mapper;
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

    }
}
