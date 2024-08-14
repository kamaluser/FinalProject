using Xunit;
using Moq;
using Cinema.Service.Implementations;
using Cinema.Data.Repositories.Interfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinema.Service.Dtos.MovieDtos;
using Cinema.Service.Exceptions;
using System.Linq;
using Cinema.Core.Entites;
using System.Linq.Expressions;

namespace CinemaApp.Tests.UnitTests
{
    public class MovieServiceTests
    {
        private readonly MovieService _movieService;
        private readonly Mock<IMovieRepository> _movieRepositoryMock;
        private readonly Mock<ILanguageRepository> _languageRepositoryMock;
        private readonly Mock<IWebHostEnvironment> _webHostEnvironmentMock;
        private readonly Mock<IMapper> _mapperMock;

        public MovieServiceTests()
        {
            _movieRepositoryMock = new Mock<IMovieRepository>();
            _languageRepositoryMock = new Mock<ILanguageRepository>();
            _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            _mapperMock = new Mock<IMapper>();

            _movieService = new MovieService(
                _movieRepositoryMock.Object,
                _languageRepositoryMock.Object,
                _webHostEnvironmentMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task GetAllMovies_ShouldReturnAllMovies()
        {
            // Arrange
            var movies = new List<Movie>
            {
                new Movie { Id = 1, Title = "Movie 1" },
                new Movie { Id = 2, Title = "Movie 2" }
            };
            _movieRepositoryMock.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Movie, bool>>>(), null))
                                .Returns(movies.AsQueryable());

            var movieDtos = new List<AdminMovieGetDto>
            {
                new AdminMovieGetDto { Id = 1, Title = "Movie 1" },
                new AdminMovieGetDto { Id = 2, Title = "Movie 2" }
            };
            _mapperMock.Setup(m => m.Map<List<AdminMovieGetDto>>(It.IsAny<List<Movie>>()))
                       .Returns(movieDtos);

            // Act
            var result = _movieService.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, m => m.Title == "Movie 1");
            Assert.Contains(result, m => m.Title == "Movie 2");
        }

        [Fact]
        public async Task Create_ShouldThrowRestException_WhenMovieAlreadyExists()
        {
            // Arrange
            var createDto = new AdminMovieCreateDto
            {
                Title = "Existing Movie",
                LanguageIds = new List<int> { 1 }
            };

            _movieRepositoryMock.Setup(repo => repo.Exists(It.IsAny<Expression<Func<Movie, bool>>>(), null))
                                .Returns(true);

            // Act & Assert
            var exception = Assert.Throws<RestException>(() => _movieService.Create(createDto));
            Assert.Equal(StatusCodes.Status400BadRequest, exception.Code);
            Assert.Contains("A movie with the given title already exists.", exception.Message);
        }

        [Fact]
        public async Task GetMovieById_ShouldReturnNull_WhenMovieDoesNotExist()
        {
            // Arrange
            _movieRepositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Movie, bool>>>(), null))
                                .Returns((Movie)null);

            // Act
            var result = _movieService.GetById(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddMovie_ShouldAddMovieSuccessfully()
        {
            // Arrange
            var createDto = new AdminMovieCreateDto
            {
                Title = "New Movie",
                LanguageIds = new List<int> { 1 }
            };

            _mapperMock.Setup(m => m.Map<Movie>(createDto))
                       .Returns(new Movie { Title = "New Movie" });

            _movieRepositoryMock.Setup(repo => repo.Add(It.IsAny<Movie>()));

            // Act
            _movieService.Create(createDto);

            // Assert
            _movieRepositoryMock.Verify(repo => repo.Add(It.Is<Movie>(m => m.Title == "New Movie")), Times.Once);
            _movieRepositoryMock.Verify(repo => repo.Save(), Times.Once);
        }
    }
}
