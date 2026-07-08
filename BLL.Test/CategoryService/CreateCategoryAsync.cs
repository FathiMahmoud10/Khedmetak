using AutoMapper;
using FluentAssertions;
using Khedmetak.BLL.DTOS.Admin;
using Khedmetak.BLL.DTOS.Categorys;
using Khedmetak.BLL.Services.Implementation;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.Abstraction.UnitOfWork;
using Khedmetak.DAL.Repositories.Interfaces;
using Moq;
using Xunit;

namespace BLL.Test.CategoryServicetest
{
    public class CategoryServiceTests
    {
        private readonly Mock<ICategoryRepository> _repoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

        [Fact]
        public async Task CreateCategoryAsync_Should_Return_CategoryDto()
        {
            var createDto = new CreateCategoryDto
            {
                Name = "Cleaning"
            };

            var category = new Category
            {
                Id = 1,
                Name = "Cleaning"
            };

            var categoryDto = new CategoryDto
            {
                Id = 1,
                Name = "Cleaning"
            };

            _mapperMock
                .Setup(x => x.Map<Category>(createDto))
                .Returns(category);

            _mapperMock
                .Setup(x => x.Map<CategoryDto>(category))
                .Returns(categoryDto);
            var service = new Khedmetak.BLL.Services.Implementation.CategoryService(
           _repoMock.Object,
           _mapperMock.Object,
           _unitOfWorkMock.Object);


            var result = await service.CreateCategoryAsync(createDto);


            result.Should().NotBeNull();
            result.Name.Should().Be("Cleaning");

            _repoMock.Verify(x => x.Add(category), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
    }
}