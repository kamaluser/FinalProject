using AutoMapper;
using Cinema.Core.Entites;
using Cinema.Data.Repositories.Implementations;
using Cinema.Data.Repositories.Interfaces;
using Cinema.Service.Dtos;
using Cinema.Service.Dtos.LanguageDtos;
using Cinema.Service.Dtos.SeatDtos;
using Cinema.Service.Dtos.SessionDtos;
using Cinema.Service.Exceptions;
using Cinema.Service.Interfaces;
using Cinema.Service.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
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
        private readonly IOrderRepository _orderRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly IMapper _mapper;
        public readonly EmailService _emailService;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public SessionService(
            ISessionRepository sessionRepository,
            IMovieRepository movieRepository,
            IHallRepository hallRepository,
            ILanguageRepository languageRepository,
            IOrderRepository orderRepository,
            ISeatRepository seatRepository,
            IMapper mapper,
            EmailService emailService,
            IWebHostEnvironment hostingEnvironment
            )
        {
            _sessionRepository = sessionRepository;
            _movieRepository = movieRepository;
            _hallRepository = hallRepository;
            _languageRepository = languageRepository;
            _orderRepository = orderRepository;
            _seatRepository = seatRepository;
            _mapper = mapper;
            _emailService = emailService;
            _hostingEnvironment = hostingEnvironment;
        }


        private bool IsTimeConflict(DateTime showDateTime, int duration, int hallId, int? excludedSessionId = null)
        {
            var sessions = _sessionRepository.GetAll(s => s.HallId == hallId && !s.IsDeleted)
                                             .Where(s => s.Id != excludedSessionId) 
                                             .ToList();

            var newSessionEnd = showDateTime.AddMinutes(duration);

            foreach (var session in sessions)
            {
                var sessionStart = session.ShowDateTime;
                var sessionEnd = session.ShowDateTime.AddMinutes(session.Duration);

                if (showDateTime < sessionEnd.AddMinutes(30) && newSessionEnd > sessionStart.AddMinutes(-30))
                {
                    return true;
                }
            }

            return false;
        }


        public List<UserSeatGetDto> GetSeatsForSession(int sessionId)
        {
            var session = _sessionRepository.Get(s => s.Id == sessionId, "Hall");

            if (session == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Session not found.");
            }

            var seats = _seatRepository.GetAll(s => s.HallId == session.HallId);
            var orders =  _orderRepository.GetAll(o => o.SessionId == sessionId);
            var orderedSeatIds = orders.SelectMany(o => o.OrderSeats.Select(os => os.SeatId)).ToList();

            var seatDtos = seats.Select(seat => new UserSeatGetDto
            {
                SeatId = seat.Id,
                SeatNumber = seat.Number,
                IsOrdered = orderedSeatIds.Contains(seat.Id)
            }).ToList();

            return seatDtos;
        }


        public List<UserSessionDetailsDto> GetSessionsByMovieAndDateAsync(int movieId, DateTime date, int? branchId = null, int? languageId = null)
        {
            var movie = _movieRepository.GetAll(x => x.Id == movieId && !x.IsDeleted)
                .Include(x => x.Sessions)
                .ThenInclude(s => s.Hall)
                .ThenInclude(h => h.Branch)
                .Include(x => x.Sessions)
                .ThenInclude(s => s.Language)
                .FirstOrDefault();

            if (movie == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Movie not found.");
            }

            var sessionsQuery = movie.Sessions
                .Where(s => s.ShowDateTime.Date == date.Date && !s.IsDeleted);

            if (branchId.HasValue)
            {
                sessionsQuery = sessionsQuery.Where(s => s.Hall.Branch.Id == branchId.Value);
            }

            if (languageId.HasValue)
            {
                sessionsQuery = sessionsQuery.Where(s => s.Language.Id == languageId.Value);
            }

            var sessions = sessionsQuery.ToList();

            var sessionDtos = _mapper.Map<List<UserSessionDetailsDto>>(sessions);

            return sessionDtos;
        }


        public async Task<int> GetSessionCountLastMonthAsync()
        {
            var now = DateTime.UtcNow;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            return await _sessionRepository.GetAll(s => s.ShowDateTime >= startDate && s.ShowDateTime <= endDate && !s.IsDeleted)
                                           .CountAsync();
        }

        public async Task<List<AdminSessionGetDto>> GetSessionsByHall(int hallId)
        {
            var sessions = _sessionRepository.GetAll(x => x.HallId == hallId && !x.IsDeleted)
                                             .Include(x => x.Movie)
                                             .Include(x => x.Language)
                                             .Include(x => x.Hall)
                                             .ThenInclude(h => h.Branch)
                                             .ToList();

            return _mapper.Map<List<AdminSessionGetDto>>(sessions);
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

            if (dto.ShowDateTime < movie.ReleaseDate)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "ShowDateTime", "The session date cannot be earlier than the movie's release date.");
            }

            var hall = _hallRepository.Get(x => x.Id == dto.HallId);
            if (hall == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Hall not found");
            }

            if (IsTimeConflict(dto.ShowDateTime, dto.Duration, dto.HallId))
            {
                throw new RestException(StatusCodes.Status400BadRequest, "ShowDateTime", "The new session conflicts with an existing session in the hall.");
            }

            var session = _mapper.Map<Session>(dto);
            session.Movie = movie;
            session.Hall = hall;

            _sessionRepository.Add(session);
            _sessionRepository.Save();

            return session.Id;
        }

        public List<AdminSessionGetDto> GetAll()
        {
            var sessions = _sessionRepository.GetAll(x => !x.IsDeleted)
                                             .Include(x => x.Movie)
                                             .Include(x => x.Language)
                                             .Include(x => x.Hall)
                                             .ThenInclude(h => h.Branch)
                                             .ToList();

            return _mapper.Map<List<AdminSessionGetDto>>(sessions);
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

            if (dto.ShowDateTime < movie.ReleaseDate)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "ShowDateTime", "The session date cannot be earlier than the movie's release date.");
            }

            if (IsTimeConflict(dto.ShowDateTime.Value, dto.Duration.Value, dto.HallId.Value, id))
            {
                throw new RestException(StatusCodes.Status400BadRequest, "ShowDateTime", "The new session conflicts with an existing session in the hall.");
            }

            if (session.ShowDateTime > DateTime.Now && dto.ShowDateTime > DateTime.Now)
            {
                var orders = _orderRepository.GetAll(o => o.SessionId == session.Id)
                                  .Include(o => o.User) 
                                  .ToList();
                foreach (var order in orders)
                {
                    if (order.User != null && !string.IsNullOrEmpty(order.User.Email))
                    {
                        var subject = "Session Updated";

                        var templatePath = Path.Combine(_hostingEnvironment.WebRootPath, "Templates", "EmailTemplates", "SessionUpdatedTemplate.html");
                        var bodyTemplate = File.ReadAllText(templatePath);

                        var body = bodyTemplate.Replace("@Model.UserName", order.User.FullName)
                                               .Replace("@Model.ShowDateTime", dto.ShowDateTime.Value.ToString("MMMM d, yyyy h:mm tt"));

                        _emailService.Send(order.User.Email, subject, body);
                    }
                }
            }


            session.Movie = movie;
            session.Hall = hall;
            session.Language = language;
            session.ShowDateTime = dto.ShowDateTime.Value;
            session.Price = dto.Price.Value;
            session.Duration = dto.Duration.Value;
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

        public List<Session> GetSessionsForReminder(DateTime currentDateTime, TimeSpan reminderWindow)
        {
            var startTime = currentDateTime;
            var endTime = currentDateTime.Add(reminderWindow);

            return _sessionRepository.GetAll(s => s.ShowDateTime > startTime && s.ShowDateTime <= endTime && !s.IsDeleted)
                                     .Include(s => s.Movie)
                                     .Include(s => s.Hall)
                                     .Include(s => s.Language)
                                     .ToList();
        }

        public async Task<List<AdminSessionLanguageDto>> GetSessionCountByLanguageThisMonthAsync()
        {
            var now = DateTime.UtcNow;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var sessions = await _sessionRepository.GetAll(s => s.ShowDateTime >= startDate && s.ShowDateTime <= endDate && !s.IsDeleted)
                                                    .Include(s => s.Language)
                                                    .ToListAsync();

            var languageSessionCount = sessions
                .GroupBy(s => s.Language.Name)
                .Select(g => new AdminSessionLanguageDto
                {
                    Language = g.Key,
                    SessionCount = g.Count()
                })
                .ToList();

            return languageSessionCount;
        }


    }
}
