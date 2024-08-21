/*using AutoMapper;
using Cinema.Core.Entites;
using Cinema.Data.Repositories.Interfaces;
using Cinema.Service.Dtos;
using Cinema.Service.Dtos.SeatDtos;
using Cinema.Service.Dtos.SessionDtos;
using Cinema.Service.Exceptions;
using Cinema.Service.Implementations;
using Cinema.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Cinema.Test.UnitTests
{
    public class SessionServiceTests
    {
        private readonly SessionService _sessionService;
        private readonly Mock<ISessionRepository> _sessionRepositoryMock;
        private readonly Mock<IMovieRepository> _movieRepositoryMock;
        private readonly Mock<IHallRepository> _hallRepositoryMock;
        private readonly Mock<ILanguageRepository> _languageRepositoryMock;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<ISeatRepository> _seatRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public SessionServiceTests()
        {
            _sessionRepositoryMock = new Mock<ISessionRepository>();
            _movieRepositoryMock = new Mock<IMovieRepository>();
            _hallRepositoryMock = new Mock<IHallRepository>();
            _languageRepositoryMock = new Mock<ILanguageRepository>();
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _seatRepositoryMock = new Mock<ISeatRepository>();
            _mapperMock = new Mock<IMapper>();

            _sessionService = new SessionService(
                _sessionRepositoryMock.Object,
                _movieRepositoryMock.Object,
                _hallRepositoryMock.Object,
                _languageRepositoryMock.Object,
                _orderRepositoryMock.Object,
                _seatRepositoryMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public void GetSeatsForSession_ShouldReturnSeats_WithCorrectOrderStatus()
        {
            // Arrange
            var sessionId = 1;
            var hallId = 1;
            var seats = new List<Seat>
            {
                new Seat { Id = 1, HallId = hallId, Number = 1 },
                new Seat { Id = 2, HallId = hallId, Number = 2 }
            };

            var orders = new List<Order>
            {
                new Order
                {
                    SessionId = sessionId,
                    OrderSeats = new List<OrderSeat>
                    {
                        new OrderSeat { SeatId = 1 }
                    }
                }
            };

            _sessionRepositoryMock.Setup(repo => repo.Get(
                It.IsAny<Expression<Func<Session, bool>>>(),
                "Hall"))
                .Returns(new Session { Id = sessionId, HallId = hallId });

            _seatRepositoryMock.Setup(repo => repo.GetAll(
                It.IsAny<Expression<Func<Seat, bool>>>(),
                null))
                .Returns(seats.AsQueryable());

            _orderRepositoryMock.Setup(repo => repo.GetAll(
                It.IsAny<Expression<Func<Order, bool>>>(),
                null))
                .Returns(orders.AsQueryable());

            // Act
            var result = _sessionService.GetSeatsForSession(sessionId);

            // Assert
            Assert.Equal(2, result.Count); // Ensure all seats are returned
            Assert.True(result.Any(s => s.SeatId == 1 && s.IsOrdered)); // Seat 1 should be ordered
            Assert.False(result.Any(s => s.SeatId == 2 && s.IsOrdered)); // Seat 2 should not be ordered
        }

        [Fact]
        public void Create_ShouldThrowRestException_WhenSessionConflicts()
        {
            // Arrange
            var dto = new AdminSessionCreateDto
            {
                MovieId = 1,
                HallId = 1,
                LanguageId = 1,
                ShowDateTime = DateTime.Now.AddDays(1),
                Duration = 120
            };

            _movieRepositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Movie, bool>>>(), "MovieLanguages"))
                                .Returns(new Movie { Id = 1, ReleaseDate = DateTime.Now.AddMonths(-1) });
            _hallRepositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Hall, bool>>>(), null))
                               .Returns(new Hall { Id = 1 });
            _languageRepositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Language, bool>>>(), null))
                                   .Returns(new Language { Id = 1 });
            _sessionRepositoryMock.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Session, bool>>>(), null))
                                  .Returns(new List<Session> { new Session { ShowDateTime = DateTime.Now, Duration = 60, HallId = 1 } }.AsQueryable());

            // Act & Assert
            var exception = Assert.Throws<RestException>(() => _sessionService.Create(dto));
            Assert.Equal(StatusCodes.Status400BadRequest, exception.Code);
            Assert.Contains("conflicts", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GetSessionCountLastMonthAsync_ShouldReturnCount()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            _sessionRepositoryMock.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Session, bool>>>(), null))
                                  .Returns(new List<Session>
                                  {
                                      new Session { ShowDateTime = now.AddDays(-5), IsDeleted = false },
                                      new Session { ShowDateTime = now.AddDays(-10), IsDeleted = false }
                                  }.AsQueryable());

            // Act
            var result = await _sessionService.GetSessionCountLastMonthAsync();

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public async Task GetSessionsByHall_ShouldReturnSessions()
        {
            // Arrange
            var hallId = 1;
            var sessions = new List<Session>
            {
                new Session { Id = 1, HallId = hallId, Movie = new Movie(), Language = new Language(), Hall = new Hall() }
            };

            _sessionRepositoryMock.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Session, bool>>>(), null))
                                  .Returns(sessions.AsQueryable());
            _mapperMock.Setup(m => m.Map<List<AdminSessionGetDto>>(It.IsAny<List<Session>>()))
                       .Returns(new List<AdminSessionGetDto>());

            // Act
            var result = await _sessionService.GetSessionsByHall(hallId);

            // Assert
            Assert.NotEmpty(result);
            Assert.IsType<List<AdminSessionGetDto>>(result);
        }

        [Fact]
        public void GetAll_ShouldReturnSessions()
        {
            // Arrange
            var sessions = new List<Session>
            {
                new Session { Id = 1, Movie = new Movie(), Language = new Language(), Hall = new Hall() }
            };

            _sessionRepositoryMock.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Session, bool>>>(), null))
                                  .Returns(sessions.AsQueryable());
            _mapperMock.Setup(m => m.Map<List<AdminSessionGetDto>>(It.IsAny<List<Session>>()))
                       .Returns(new List<AdminSessionGetDto>());

            // Act
            var result = _sessionService.GetAll();

            // Assert
            Assert.NotEmpty(result);
            Assert.IsType<List<AdminSessionGetDto>>(result);
        }

        [Fact]
        public void GetAllByPage_ShouldReturnPaginatedSessions()
        {
            // Arrange
            var sessions = new List<Session>
            {
                new Session { Id = 1, Movie = new Movie(), Language = new Language(), Hall = new Hall() }
            }.AsQueryable();

            _sessionRepositoryMock.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Session, bool>>>(), null))
                                  .Returns(sessions);
            _mapperMock.Setup(m => m.Map<List<AdminSessionGetDto>>(It.IsAny<List<Session>>()))
                       .Returns(new List<AdminSessionGetDto>());

            var paginatedList = PaginatedList<Session>.Create(sessions, 1, 10);
            var paginatedDtoList = new PaginatedList<AdminSessionGetDto>(
                paginatedList.Items.Select(item => _mapperMock.Object.Map<AdminSessionGetDto>(item)).ToList(),
                paginatedList.TotalPages,
                paginatedList.PageIndex,
                paginatedList.PageSize
            );

            // Act
            var result = _sessionService.GetAllByPage();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(paginatedDtoList.TotalPages, result.TotalPages);
            Assert.IsType<PaginatedList<AdminSessionGetDto>>(result);
        }

        [Fact]
        public void GetById_ShouldReturnSession()
        {
            // Arrange
            var sessionId = 1;
            var session = new Session
            {
                Id = sessionId,
                Movie = new Movie(),
                Language = new Language(),
                Hall = new Hall { Branch = new Branch() }
            };

            _sessionRepositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Session, bool>>>(), "Movie, Hall, Language, Hall.Branch"))
                                  .Returns(session);
            _mapperMock.Setup(m => m.Map<AdminSessionGetDto>(It.IsAny<Session>()))
                       .Returns(new AdminSessionGetDto());

            // Act
            var result = _sessionService.GetById(sessionId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<AdminSessionGetDto>(result);
        }
    }
}
using AutoMapper;
using Cinema.Core.Entites;
using Cinema.Data.Repositories.Interfaces;
using Cinema.Service.Dtos;
using Cinema.Service.Dtos.SeatDtos;
using Cinema.Service.Dtos.SessionDtos;
using Cinema.Service.Exceptions;
using Cinema.Service.Implementations;
using Cinema.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Cinema.Test.UnitTests
{
    public class SessionServiceTests
    {
        private readonly SessionService _sessionService;
        private readonly Mock<ISessionRepository> _sessionRepositoryMock;
        private readonly Mock<IMovieRepository> _movieRepositoryMock;
        private readonly Mock<IHallRepository> _hallRepositoryMock;
        private readonly Mock<ILanguageRepository> _languageRepositoryMock;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<ISeatRepository> _seatRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public SessionServiceTests()
        {
            _sessionRepositoryMock = new Mock<ISessionRepository>();
            _movieRepositoryMock = new Mock<IMovieRepository>();
            _hallRepositoryMock = new Mock<IHallRepository>();
            _languageRepositoryMock = new Mock<ILanguageRepository>();
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _seatRepositoryMock = new Mock<ISeatRepository>();
            _mapperMock = new Mock<IMapper>();

            _sessionService = new SessionService(
                _sessionRepositoryMock.Object,
                _movieRepositoryMock.Object,
                _hallRepositoryMock.Object,
                _languageRepositoryMock.Object,
                _orderRepositoryMock.Object,
                _seatRepositoryMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public void GetSeatsForSession_ShouldReturnSeats_WithCorrectOrderStatus()
        {
            // Arrange
            var sessionId = 1;
            var hallId = 1;
            var seats = new List<Seat>
            {
                new Seat { Id = 1, HallId = hallId, Number = 1 },
                new Seat { Id = 2, HallId = hallId, Number = 2 }
            };

            var orders = new List<Order>
            {
                new Order
                {
                    SessionId = sessionId,
                    OrderSeats = new List<OrderSeat>
                    {
                        new OrderSeat { SeatId = 1 }
                    }
                }
            };

            _sessionRepositoryMock.Setup(repo => repo.Get(
                It.IsAny<Expression<Func<Session, bool>>>(),
                "Hall"))
                .Returns(new Session { Id = sessionId, HallId = hallId });

            _seatRepositoryMock.Setup(repo => repo.GetAll(
                It.IsAny<Expression<Func<Seat, bool>>>(),
                null))
                .Returns(seats.AsQueryable());

            _orderRepositoryMock.Setup(repo => repo.GetAll(
                It.IsAny<Expression<Func<Order, bool>>>(),
                null))
                .Returns(orders.AsQueryable());

            // Act
            var result = _sessionService.GetSeatsForSession(sessionId);

            // Assert
            Assert.Equal(2, result.Count); // Ensure all seats are returned
            Assert.True(result.Any(s => s.SeatId == 1 && s.IsOrdered)); // Seat 1 should be ordered
            Assert.False(result.Any(s => s.SeatId == 2 && s.IsOrdered)); // Seat 2 should not be ordered
        }

        [Fact]
        public void Create_ShouldThrowRestException_WhenSessionConflicts()
        {
            // Arrange
            var dto = new AdminSessionCreateDto
            {
                MovieId = 1,
                HallId = 1,
                LanguageId = 1,
                ShowDateTime = DateTime.Now.AddDays(1),
                Duration = 120
            };

            _movieRepositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Movie, bool>>>(), "MovieLanguages"))
                                .Returns(new Movie { Id = 1, ReleaseDate = DateTime.Now.AddMonths(-1) });
            _hallRepositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Hall, bool>>>(), null))
                               .Returns(new Hall { Id = 1 });
            _languageRepositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Language, bool>>>(), null))
                                   .Returns(new Language { Id = 1 });
            _sessionRepositoryMock.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Session, bool>>>(), null))
                                  .Returns(new List<Session> { new Session { ShowDateTime = DateTime.Now, Duration = 60, HallId = 1 } }.AsQueryable());

            // Act & Assert
            var exception = Assert.Throws<RestException>(() => _sessionService.Create(dto));
            Assert.Equal(StatusCodes.Status400BadRequest, exception.Code);
            Assert.Contains("conflicts", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GetSessionCountLastMonthAsync_ShouldReturnCount()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            _sessionRepositoryMock.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Session, bool>>>(), null))
                                  .Returns(new List<Session>
                                  {
                                      new Session { ShowDateTime = now.AddDays(-5), IsDeleted = false },
                                      new Session { ShowDateTime = now.AddDays(-10), IsDeleted = false }
                                  }.AsQueryable());

            // Act
            var result = await _sessionService.GetSessionCountLastMonthAsync();

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public async Task GetSessionsByHall_ShouldReturnSessions()
        {
            // Arrange
            var hallId = 1;
            var sessions = new List<Session>
            {
                new Session { Id = 1, HallId = hallId, Movie = new Movie(), Language = new Language(), Hall = new Hall() }
            };

            _sessionRepositoryMock.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Session, bool>>>(), null))
                                  .Returns(sessions.AsQueryable());
            _mapperMock.Setup(m => m.Map<List<AdminSessionGetDto>>(It.IsAny<List<Session>>()))
                       .Returns(new List<AdminSessionGetDto>());

            // Act
            var result = await _sessionService.GetSessionsByHall(hallId);

            // Assert
            Assert.NotEmpty(result);
            Assert.IsType<List<AdminSessionGetDto>>(result);
        }

        [Fact]
        public void GetAll_ShouldReturnSessions()
        {
            // Arrange
            var sessions = new List<Session>
            {
                new Session { Id = 1, Movie = new Movie(), Language = new Language(), Hall = new Hall() }
            };

            _sessionRepositoryMock.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Session, bool>>>(), null))
                                  .Returns(sessions.AsQueryable());
            _mapperMock.Setup(m => m.Map<List<AdminSessionGetDto>>(It.IsAny<List<Session>>()))
                       .Returns(new List<AdminSessionGetDto>());

            // Act
            var result = _sessionService.GetAll();

            // Assert
            Assert.NotEmpty(result);
            Assert.IsType<List<AdminSessionGetDto>>(result);
        }

        [Fact]
        public void GetAllByPage_ShouldReturnPaginatedSessions()
        {
            // Arrange
            var sessions = new List<Session>
            {
                new Session { Id = 1, Movie = new Movie(), Language = new Language(), Hall = new Hall() }
            }.AsQueryable();

            _sessionRepositoryMock.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Session, bool>>>(), null))
                                  .Returns(sessions);
            _mapperMock.Setup(m => m.Map<List<AdminSessionGetDto>>(It.IsAny<List<Session>>()))
                       .Returns(new List<AdminSessionGetDto>());

            var paginatedList = PaginatedList<Session>.Create(sessions, 1, 10);
            var paginatedDtoList = new PaginatedList<AdminSessionGetDto>(
                paginatedList.Items.Select(item => _mapperMock.Object.Map<AdminSessionGetDto>(item)).ToList(),
                paginatedList.TotalPages,
                paginatedList.PageIndex,
                paginatedList.PageSize
            );

            // Act
            var result = _sessionService.GetAllByPage();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(paginatedDtoList.TotalPages, result.TotalPages);
            Assert.IsType<PaginatedList<AdminSessionGetDto>>(result);
        }

        [Fact]
        public void GetById_ShouldReturnSession()
        {
            // Arrange
            var sessionId = 1;
            var session = new Session
            {
                Id = sessionId,
                Movie = new Movie(),
                Language = new Language(),
                Hall = new Hall { Branch = new Branch() }
            };

            _sessionRepositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Session, bool>>>(), "Movie, Hall, Language, Hall.Branch"))
                                  .Returns(session);
            _mapperMock.Setup(m => m.Map<AdminSessionGetDto>(It.IsAny<Session>()))
                       .Returns(new AdminSessionGetDto());

            // Act
            var result = _sessionService.GetById(sessionId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<AdminSessionGetDto>(result);
        }
    }
}
*/