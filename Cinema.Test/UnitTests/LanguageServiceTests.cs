using AutoMapper;
using Cinema.Core.Entites;
using Cinema.Data.Repositories.Interfaces;
using Cinema.Service.Dtos;
using Cinema.Service.Dtos.LanguageDtos;
using Cinema.Service.Exceptions;
using Cinema.Service.Implementations;
using Cinema.Service.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace Cinema.Test.UnitTests
{
    public class LanguageServiceTests
    {
        private readonly LanguageService _languageService;
        private readonly Mock<ILanguageRepository> _languageRepositoryMock;
        private readonly Mock<IWebHostEnvironment> _environmentMock;
        private readonly Mock<IMapper> _mapperMock;

        public LanguageServiceTests()
        {
            _languageRepositoryMock = new Mock<ILanguageRepository>();
            _environmentMock = new Mock<IWebHostEnvironment>();
            _mapperMock = new Mock<IMapper>();

            _languageService = new LanguageService(
                _languageRepositoryMock.Object,
                _environmentMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public void Create_ShouldThrowRestException_WhenLanguageExists()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            var content = "This is a dummy file";
            var fileName = "flag.png";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;

            mockFile.Setup(f => f.OpenReadStream()).Returns(ms);
            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.Length).Returns(ms.Length);

            var createDto = new AdminLanguageCreateDto
            {
                Name = "Mozambik",
                FlagPhoto = mockFile.Object
            };

            _languageRepositoryMock.Setup(repo => repo.Exists(It.IsAny<Expression<Func<Language, bool>>>()))
                                   .Returns(true);

            // Act
            var exception = Assert.Throws<RestException>(() => _languageService.Create(createDto));

            Assert.Equal(StatusCodes.Status400BadRequest, exception.Code);
            Assert.Contains("Language by given Name already exists.", exception.Message);

            _languageRepositoryMock.Verify(repo => repo.Exists(It.IsAny<Expression<Func<Language, bool>>>()), Times.Once);

            _languageRepositoryMock.Verify(repo => repo.Add(It.IsAny<Language>()), Times.Never);
            _languageRepositoryMock.Verify(repo => repo.Save(), Times.Never);
        }

        [Fact]
        public void Edit_ShouldThrowRestException_WhenLanguageNotFound()
        {
            // Arrange
            var editDto = new AdminLanguageEditDto { Name = "English" };
            _languageRepositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Language, bool>>>()))
                                   .Returns((Language)null);

            // Act & Assert
            var exception = Assert.Throws<RestException>(() => _languageService.Edit(1, editDto));
            Assert.Equal(StatusCodes.Status404NotFound, exception.Code);
            Assert.Contains("Language not found", exception.Message);
        }


        [Fact]
        public void Delete_ShouldThrowRestException_WhenLanguageNotFound()
        {
            // Arrange
            _languageRepositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Language, bool>>>()))
                                   .Returns((Language)null);

            // Act & Assert
            var exception = Assert.Throws<RestException>(() => _languageService.Delete(1));
            Assert.Equal(StatusCodes.Status404NotFound, exception.Code);
            Assert.Contains("Language not found", exception.Message);
        }

        [Fact]
        public void Delete_ShouldRemoveLanguage_AndFlagPhoto_WhenValid()
        {
            // Arrange
            var language = new Language
            {
                Id = 1,
                Name = "English",
                FlagPhoto = "flag.png"
            };

            _languageRepositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Language, bool>>>()))
                                   .Returns(language);
            _environmentMock.Setup(e => e.WebRootPath).Returns("wwwroot");

            // Act
            _languageService.Delete(1);

            // Assert
            _languageRepositoryMock.Verify(repo => repo.Delete(It.IsAny<Language>()), Times.Once);
            _languageRepositoryMock.Verify(repo => repo.Save(), Times.Once);
        }

        [Fact]
        public void GetById_ShouldThrowRestException_WhenLanguageNotFound()
        {
            // Arrange
            _languageRepositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Language, bool>>>()))
                                   .Returns((Language)null);

            // Act & Assert
            var exception = Assert.Throws<RestException>(() => _languageService.GetById(1));
            Assert.Equal(StatusCodes.Status404NotFound, exception.Code);
            Assert.Contains("Language not found", exception.Message);
        }

        [Fact]
        public void GetById_ShouldReturnLanguage_WhenValid()
        {
            // Arrange
            var language = new Language { Id = 1, Name = "English" };
            _languageRepositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Language, bool>>>()))
                                   .Returns(language);
            _mapperMock.Setup(m => m.Map<AdminLanguageGetDto>(language))
                       .Returns(new AdminLanguageGetDto { Id = 1, Name = "English" });

            // Act
            var result = _languageService.GetById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("English", result.Name);
        }

        [Fact]
        public void GetAll_ShouldReturnAllLanguages()
        {
            // Arrange
            var languages = new List<Language>
            {
                new Language { Id = 1, Name = "English" },
                new Language { Id = 2, Name = "Spanish" }
            };

            _languageRepositoryMock.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Language, bool>>>()))
                                   .Returns(languages.AsQueryable());
            _mapperMock.Setup(m => m.Map<List<AdminLanguageGetDto>>(It.IsAny<List<Language>>()))
                       .Returns(new List<AdminLanguageGetDto>
                       {
                           new AdminLanguageGetDto { Id = 1, Name = "English" },
                           new AdminLanguageGetDto { Id = 2, Name = "Spanish" }
                       });

            // Act
            var result = _languageService.GetAll();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetAllByPage_ShouldReturnPaginatedLanguages()
        {
            // Arrange
            var languages = new List<Language>
            {
                new Language { Id = 1, Name = "English" },
                new Language { Id = 2, Name = "Spanish" }
            }.AsQueryable();

            _languageRepositoryMock.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Language, bool>>>()))
                                   .Returns(languages);

            var paginatedList = PaginatedList<Language>.Create(languages, 1, 10);
            var paginatedDtoList = new PaginatedList<AdminLanguageGetDto>(
                paginatedList.Items.Select(item => _mapperMock.Object.Map<AdminLanguageGetDto>(item)).ToList(),
                paginatedList.TotalPages,
                paginatedList.PageIndex,
                paginatedList.PageSize
            );

            // Act
            var result = _languageService.GetAllByPage();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(paginatedDtoList.TotalPages, result.TotalPages);
            Assert.IsType<PaginatedList<AdminLanguageGetDto>>(result);
        }
    }
}