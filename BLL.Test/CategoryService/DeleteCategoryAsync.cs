using AutoMapper;
using FluentAssertions;
using Khedmetak.BLL.Services.Implementation;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.Abstraction.UnitOfWork;
using Khedmetak.DAL.Repositories.Interfaces;
using Moq;
using Xunit;
namespace BLL.Test.CategoryService
{
    public class DeleteCategoryAsyncTests
{
    [Fact]
    public async Task Should_Delete_Category()
    {
        var repo = new Mock<ICategoryRepository>();
        var mapper = new Mock<IMapper>();
        var unit = new Mock<IUnitOfWork>();

        var category = new Category
        {
            Id = 1,
            Name = "Cleaning"
        };

        repo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(category);

        var service = new Khedmetak.BLL.Services.Implementation.CategoryService(repo.Object, mapper.Object, unit.Object);

        var result = await service.DeleteCategoryAsync(1);

        result.Should().BeTrue();

        repo.Verify(r => r.Delete(category), Times.Once);
        unit.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Should_Return_False_When_Category_NotFound()
    {
        var repo = new Mock<ICategoryRepository>();
        var mapper = new Mock<IMapper>();
        var unit = new Mock<IUnitOfWork>();

        repo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((Category?)null);

        var service = new Khedmetak.BLL.Services.Implementation.CategoryService(repo.Object, mapper.Object, unit.Object);

        var result = await service.DeleteCategoryAsync(1);

        result.Should().BeFalse();

        repo.Verify(r => r.Delete(It.IsAny<Category>()), Times.Never);
        unit.Verify(u => u.SaveChangesAsync(), Times.Never);
    }
}
}