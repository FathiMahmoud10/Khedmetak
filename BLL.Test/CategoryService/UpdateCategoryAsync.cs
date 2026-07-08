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
namespace BLL.Test.CategoryService
{
    public class UpdateCategoryAsyncTests
{
    [Fact]
    public async Task Should_Update_Category()
    {
        var repo = new Mock<ICategoryRepository>();
        var mapper = new Mock<IMapper>();
        var unit = new Mock<IUnitOfWork>();

        var category = new Category
        {
            Id = 1,
            Name = "Old"
        };

        var dto = new UpdateCategoryDto
        {
            Name = "New"
        };

        var resultDto = new CategoryDto
        {
            Id = 1,
            Name = "New"
        };

        repo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(category);

        mapper.Setup(m => m.Map<CategoryDto>(category))
              .Returns(resultDto);

        var service = new Khedmetak.BLL.Services.Implementation.CategoryService(repo.Object, mapper.Object, unit.Object);

        var result = await service.UpdateCategoryAsync(1, dto);

        result.Should().NotBeNull();
        result!.Name.Should().Be("New");

        repo.Verify(r => r.Update(category), Times.Once);
        unit.Verify(u => u.SaveChangesAsync(), Times.Once);
    }


    [Fact]
    public async Task Should_Return_Null_When_Category_NotFound()
    {
        var repo = new Mock<ICategoryRepository>();
        var mapper = new Mock<IMapper>();
        var unit = new Mock<IUnitOfWork>();

        repo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((Category?)null);

        var service = new Khedmetak.BLL.Services.Implementation.CategoryService(repo.Object, mapper.Object, unit.Object);

        var result = await service.UpdateCategoryAsync(1, new UpdateCategoryDto());

        result.Should().BeNull();

        repo.Verify(r => r.Update(It.IsAny<Category>()), Times.Never);
        unit.Verify(u => u.SaveChangesAsync(), Times.Never);
    }
}
}