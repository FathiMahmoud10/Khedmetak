using FluentAssertions;
using Khedmetak.BLL.Services.Implementation;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.Abstraction.UnitOfWork;
using Khedmetak.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace BLL.Test.StatisticsServiceTest
{
    public class StatisticsServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitMock = new();
        private readonly Mock<IGovServiceRepository> _govRepoMock = new();
        private readonly Mock<ICategoryRepository> _categoryRepoMock = new();


        private StatisticsService CreateService(int usersCount)
        {
            // Mock UserManager
            var userStoreMock = new Mock<IUserStore<User>>();

            var userManager = new Mock<UserManager<User>>(
                userStoreMock.Object,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!);


            var users = new List<User>();

            for (int i = 0; i < usersCount; i++)
            {
                users.Add(new User());
            }


            userManager
                .Setup(x => x.Users)
                .Returns(users.AsQueryable());


            _unitMock
                .Setup(x => x.GovServices)
                .Returns(_govRepoMock.Object);


            _unitMock
                .Setup(x => x.Categories)
                .Returns(_categoryRepoMock.Object);



            return new StatisticsService(
                userManager.Object,
                _unitMock.Object);
        }



        [Fact]
        public async Task GetStatisticsAsync_Should_Return_Correct_Counts()
        {
            // Arrange

            var service = CreateService(5);


            var services = new List<GovService>
            {
                new GovService(),
                new GovService(),
                new GovService()
            };


            var categories = new List<Category>
            {
                new Category(),
                new Category()
            };


            _govRepoMock
                .Setup(x => x.GetAllWithCategoryAsync())
                .ReturnsAsync(services);



            _categoryRepoMock
                .Setup(x => x.GetAllWithServicesCountAsync())
                .ReturnsAsync(categories);



            // Act

            var result = await service.GetStatisticsAsync();



            // Assert

            result.Should().NotBeNull();

            result.TotalUsers
                .Should()
                .Be(5);


            result.TotalServices
                .Should()
                .Be(3);


            result.TotalCategories
                .Should()
                .Be(2);
        }
    }
}