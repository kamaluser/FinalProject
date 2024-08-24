using AutoMapper;
using Cinema.Core.Entites;
using Cinema.Data.Repositories.Interfaces;
using Cinema.Service.Dtos;
using Cinema.Service.Dtos.BranchDtos;
using Cinema.Service.Exceptions;
using Cinema.Service.Implementations;
using Cinema.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace Cinema.Test.UnitTests
{
    public class BranchServiceTests
    {
        private readonly Mock<IBranchRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly BranchService _branchService;

        public BranchServiceTests()
        {
            _repositoryMock = new Mock<IBranchRepository>();
            _mapperMock = new Mock<IMapper>();
            _branchService = new BranchService(_repositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public void Create_ShouldThrowRestException_WhenBranchExists()
        {
            // Arrange
            var dto = new AdminBranchCreateDto { Name = "Existing Branch" };
            _repositoryMock.Setup(repo => repo.Exists(It.IsAny<Expression<Func<Branch, bool>>>())).Returns(true);

            // Act & Assert
            var exception = Assert.Throws<RestException>(() => _branchService.Create(dto));
            Assert.Equal(StatusCodes.Status400BadRequest, exception.Code);
            Assert.Equal("Branch already exists.", exception.Message);
        }

        [Fact]
        public void Create_ShouldAddBranch_WhenValid()
        {
            // Arrange
            var dto = new AdminBranchCreateDto { Name = "New Branch" };
            var branch = new Branch { Id = 1, Name = "New Branch" };
            _mapperMock.Setup(m => m.Map<Branch>(dto)).Returns(branch);

            // Act
            _branchService.Create(dto);

            // Assert
            _repositoryMock.Verify(repo => repo.Add(branch), Times.Once);
            _repositoryMock.Verify(repo => repo.Save(), Times.Once);
        }

        [Fact]
        public void GetAll_ShouldReturnBranches()
        {
            // Arrange
            var branches = new List<Branch>
            {
                new Branch { Id = 1, Name = "Branch1", IsDeleted = false },
                new Branch { Id = 2, Name = "Branch2", IsDeleted = false }
            };

            var dtos = branches.Select(b => new AdminBranchGetDto { Id = b.Id, Name = b.Name }).ToList();

            _repositoryMock.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Branch, bool>>>(), null))
                .Returns(branches.AsQueryable());

            _mapperMock.Setup(m => m.Map<List<AdminBranchGetDto>>(It.IsAny<List<Branch>>()))
                .Returns(dtos);

            // Act
            var result = _branchService.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Branch1", result[0].Name);
        }




        [Fact]
        public void GetById_ShouldReturnBranch_WhenValidId()
        {
            // Arrange
            var branch = new Branch { Id = 1, Name = "Branch1" };
            _repositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Branch, bool>>>())).Returns(branch);
            var dto = new AdminBranchGetDto { Id = branch.Id, Name = branch.Name };
            _mapperMock.Setup(m => m.Map<AdminBranchGetDto>(branch)).Returns(dto);

            // Act
            var result = _branchService.GetById(1);

            // Assert
            Assert.Equal("Branch1", result.Name);
        }

        [Fact]
        public void Edit_ShouldThrowRestException_WhenBranchExists()
        {
            // Arrange
            var updateDto = new AdminBranchEditDto { Name = "Existing Branch" };
            var branch = new Branch { Id = 1, Name = "Old Branch" };
            _repositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Branch, bool>>>())).Returns(branch);
            _repositoryMock.Setup(repo => repo.Exists(It.IsAny<Expression<Func<Branch, bool>>>())).Returns(true);

            // Act & Assert
            var exception = Assert.Throws<RestException>(() => _branchService.Edit(1, updateDto));
            Assert.Equal(StatusCodes.Status400BadRequest, exception.Code);
            Assert.Equal("Branch by this name already exists.", exception.Message);
        }

        [Fact]
        public void Delete_ShouldMarkBranchAsDeleted()
        {
            // Arrange
            var branch = new Branch { Id = 1, Name = "Branch1", IsDeleted = false };
            _repositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Branch, bool>>>())).Returns(branch);

            // Act
            _branchService.Delete(1);

            // Assert
            Assert.True(branch.IsDeleted);
            _repositoryMock.Verify(repo => repo.Save(), Times.Once);
        }
    }
}
