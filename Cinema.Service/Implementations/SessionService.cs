using AutoMapper;
using Cinema.Core.Entites;
using Cinema.Data.Repositories.Implementations;
using Cinema.Data.Repositories.Interfaces;
using Cinema.Service.Dtos;
using Cinema.Service.Dtos.SessionDtos;
using Cinema.Service.Exceptions;
using Cinema.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema.Service.Implementations
{
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IHallRepository _hallRepository;
        private readonly ILanguageRepository _languageRepository;
        private readonly IMapper _mapper;

        public SessionService(
            ISessionRepository sessionRepository,
            IMovieRepository movieRepository,
            IHallRepository hallRepository,
            ILanguageRepository languageRepository,
            IMapper mapper)
        {
            _sessionRepository = sessionRepository;
            _movieRepository = movieRepository;
            _hallRepository = hallRepository;
            _languageRepository = languageRepository;
            _mapper = mapper;
        }

        public int Create(AdminSessionCreateDto dto)
        {
            var movie = _movieRepository.Get(x => x.Id == dto.MovieId, "MovieLanguages");
            if (movie == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Movie not found");
            }

            var language = _languageRepository.Get(x => x.Id == dto.LanguageId);
            if (language == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Language not found");
            }

            if (movie.MovieLanguages == null || !movie.MovieLanguages.Any())
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Language", "This movie has no associated languages.");
            }

            var movieLanguage = movie.MovieLanguages.FirstOrDefault(x => x.LanguageId == dto.LanguageId);
            if (movieLanguage == null)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Language", "The selected language is not associated with this movie.");
            }

            var hall = _hallRepository.Get(x => x.Id == dto.HallId);
            if (hall == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Hall not found");
            }

            var session = _mapper.Map<Session>(dto);
            session.Movie = movie;
            session.Hall = hall;

            _sessionRepository.Add(session);
            _sessionRepository.Save();

            return session.Id;
        }

        public PaginatedList<AdminSessionGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {
            IQueryable<Session> query = _sessionRepository.GetAll(x => !x.IsDeleted);

            query = query.Include(x => x.Movie)
                         .Include(x => x.Language)
                         .Include(x => x.Hall)
                         .ThenInclude(h => h.Branch);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Movie.Title.Contains(search) || x.Hall.Name.Contains(search));
            }

            var paginated = PaginatedList<Session>.Create(query, page, size);

            if (paginated.TotalPages == 0)
            {
                page = 1;
            }

            var sessionsDto = _mapper.Map<List<AdminSessionGetDto>>(paginated.Items);

            return new PaginatedList<AdminSessionGetDto>(sessionsDto, paginated.TotalPages, page, size);
        }

        public AdminSessionGetDto GetById(int id)
        {
            var session = _sessionRepository.Get(
                x => x.Id == id && !x.IsDeleted,
                "Movie, Hall, Language, Hall.Branch"); 

            if (session == null)
                throw new RestException(StatusCodes.Status404NotFound, $"Session with ID {id} not found.");

            return _mapper.Map<AdminSessionGetDto>(session);
        }

        public void Edit(int id, AdminSessionEditDto dto)
        {
            var session = _sessionRepository.Get(x => x.Id == id && !x.IsDeleted);
            if (session == null)
                throw new RestException(StatusCodes.Status404NotFound, "Session not found");

            var movie = _movieRepository.GetAll(x => x.Id == dto.MovieId)
                .Include(x => x.MovieLanguages)
                .FirstOrDefault();

            if (movie == null)
                throw new RestException(StatusCodes.Status404NotFound, "Movie not found");

            if (movie.MovieLanguages == null || !movie.MovieLanguages.Any())
                throw new RestException(StatusCodes.Status400BadRequest, "Language", "The movie has no associated languages.");

            var language = _languageRepository.Get(x => x.Id == dto.LanguageId);
            if (language == null)
                throw new RestException(StatusCodes.Status404NotFound, "Language not found");

            var movieLanguage = movie.MovieLanguages.FirstOrDefault(x => x.LanguageId == dto.LanguageId);
            if (movieLanguage == null)
                throw new RestException(StatusCodes.Status400BadRequest, "Language", "The selected language is not associated with this movie.");

            var hall = _hallRepository.Get(x => x.Id == dto.HallId);
            if (hall == null)
                throw new RestException(StatusCodes.Status404NotFound, "Hall not found");

            session.Movie = movie;
            session.Hall = hall;
            session.ShowDateTime = dto.ShowDateTime;
            session.Price = dto.Price;
            session.Duration = dto.Duration;
            session.ModifiedAt = DateTime.Now;

            _sessionRepository.Save();
        }

        public void Delete(int id)
        {
            var session = _sessionRepository.Get(x => x.Id == id);
            if (session == null)
                throw new RestException(StatusCodes.Status404NotFound, "Session not found");

            session.IsDeleted = true;
            session.ModifiedAt = DateTime.Now;
            _sessionRepository.Save();
        }
    }
}
