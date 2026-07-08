using AutoMapper;
using FluentAssertions;
using Khedmetak.BLL.DTOS.Categorys;
using Khedmetak.BLL.Services.Implementation;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.Abstraction.UnitOfWork;
using Khedmetak.DAL.Repositories.Interfaces;
using Moq;
using Xunit;
namespace BLL.Test.CategoryService
{
    public class GetAllCategoriesAsyncTests
{
    [Fact]
    public async Task Should_Return_All_Categories()
    {
        // Arrange
        var repo = new Mock<ICategoryRepository>();
        var mapper = new Mock<IMapper>();
        var unit = new Mock<IUnitOfWork>();

        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Cleaning" },
            new Category { Id = 2, Name = "Painting" }
        };

        var dtos = new List<CategoryDto>
        {
            new CategoryDto { Id = 1, Name = "Cleaning" },
            new CategoryDto { Id = 2, Name = "Painting" }
        };

        repo.Setup(r => r.GetAllWithServicesCountAsync())
            .ReturnsAsync(categories);

        mapper.Setup(m => m.Map<IEnumerable<CategoryDto>>(categories))
              .Returns(dtos);

        var service = new Khedmetak.BLL.Services.Implementation.CategoryService(repo.Object, mapper.Object, unit.Object);

        // Act
        var result = await service.GetAllCategoriesAsync();

        // Assert
        result.Should().HaveCount(2);

        repo.Verify(r => r.GetAllWithServicesCountAsync(), Times.Once);
    }
}
}