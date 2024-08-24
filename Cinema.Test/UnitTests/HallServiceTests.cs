using AutoMapper;
using Cinema.Core.Entites;
using Cinema.Data.Repositories.Interfaces;
using Cinema.Service.Dtos;
using Cinema.Service.Implementations;
using Cinema.Service.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Cinema.Service.Dtos.HallDtos;
using System.Linq.Expressions;

namespace Cinema.Test.UnitTests
{
    public class HallServiceTests
    {
        private readonly Mock<IHallRepository> _hallRepositoryMock;
        private readonly Mock<IBranchRepository> _branchRepositoryMock;
        private readonly Mock<ISeatRepository> _seatRepositoryMock;
        private readonly Mock<IWebHostEnvironment> _environmentMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly HallService _hallService;

        public HallServiceTests()
        {
            _hallRepositoryMock = new Mock<IHallRepository>();
            _branchRepositoryMock = new Mock<IBranchRepository>();
            _seatRepositoryMock = new Mock<ISeatRepository>();
            _environmentMock = new Mock<IWebHostEnvironment>();
            _mapperMock = new Mock<IMapper>();

            _hallService = new HallService(
                _hallRepositoryMock.Object,
                _branchRepositoryMock.Object,
                _seatRepositoryMock.Object,
                _environmentMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public void Create_ShouldThrowRestException_WhenHallExists()
        {
            // Arrange
            var createDto = new AdminHallCreateDto
            {
                Name = "Hall1",
                BranchId = 1
            };

            _hallRepositoryMock.Setup(repo => repo.Exists(It.IsAny<Expression<Func<Hall, bool>>>()))
                .Returns(true);

            // Act & Assert
            var exception = Assert.Throws<RestException>(() => _hallService.Create(createDto));
            Assert.Equal(StatusCodes.Status400BadRequest, exception.Code);
            Assert.Equal("A hall with the given name already exists in this branch.", exception.Message);
        }

        [Fact]
        public void Create_ShouldAddHall_WhenValid()
        {
            // Arrange
            var createDto = new AdminHallCreateDto
            {
                Name = "New Hall",
                BranchId = 1
            };

            var branch = new Branch { Id = 1, IsDeleted = false };
            _branchRepositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Branch, bool>>>(), "Halls"))
                .Returns(branch);

            var hall = new Hall();
            _mapperMock.Setup(m => m.Map<Hall>(createDto)).Returns(hall);

            // Act
            _hallService.Create(createDto);

            // Assert
            _hallRepositoryMock.Verify(repo => repo.Add(hall), Times.Once);
            _hallRepositoryMock.Verify(repo => repo.Save(), Times.Once);
        }

        [Fact]
        public void GetAll_ShouldReturnListOfHalls()
        {
            // Arrange
            var halls = new List<Hall>
            {
                new Hall { Id = 1, Name = "Hall1", IsDeleted = false },
                new Hall { Id = 2, Name = "Hall2", IsDeleted = false }
            };
            _hallRepositoryMock.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Hall, bool>>>(), "Branch"))
                .Returns(halls.AsQueryable());

            // Act
            var result = _hallService.GetAll();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetById_ShouldReturnHall_WhenValidId()
        {
            // Arrange
            var hall = new Hall { Id = 1, Name = "Hall1", IsDeleted = false };
            _hallRepositoryMock.Setup(repo => repo.Get(h => h.Id == 1 && !h.IsDeleted, "Branch"))
                               .Returns(hall);

            var hallDto = new AdminHallGetDto { Id = 1, Name = "Hall1" };
            _mapperMock.Setup(m => m.Map<AdminHallGetDto>(hall))
                       .Returns(hallDto);

            // Act
            var result = _hallService.GetById(1);

            // Assert
            Assert.NotNull(result); 
            Assert.Equal("Hall1", result.Name);
        }



        [Fact]
        public void Edit_ShouldUpdateHall_WhenValid()
        {
            // Arrange
            var editDto = new AdminHallEditDto
            {
                Name = "Updated Hall",
                BranchId = 1
            };

            var hall = new Hall { Id = 1, Name = "Old Hall" };
            _hallRepositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Hall, bool>>>())).Returns(hall);

            var branch = new Branch { Id = 1, IsDeleted = false };
            _branchRepositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Branch, bool>>>(), "Halls"))
                .Returns(branch);

            _mapperMock.Setup(m => m.Map(It.IsAny<AdminHallEditDto>(), It.IsAny<Hall>())).Callback<AdminHallEditDto, Hall>((dto, entity) =>
            {
                entity.Name = dto.Name;
            });

            // Act
            _hallService.Edit(1, editDto);

            // Assert
            Assert.Equal("Updated Hall", hall.Name);
            _hallRepositoryMock.Verify(repo => repo.Save(), Times.Once);
        }


        [Fact]
        public void Delete_ShouldMarkHallAsDeleted()
        {
            // Arrange
            var hall = new Hall { Id = 1, IsDeleted = false };
            _hallRepositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Hall, bool>>>())).Returns(hall);

            // Act
            _hallService.Delete(1);

            // Assert
            Assert.True(hall.IsDeleted);
            _hallRepositoryMock.Verify(repo => repo.Save(), Times.Once);
        }
    }
}
