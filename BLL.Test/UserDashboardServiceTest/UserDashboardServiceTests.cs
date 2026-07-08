using FluentAssertions;
using Khedmetak.BLL.DTOS.UserDashboard;
using Khedmetak.BLL.Services.Implementation;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Enums;
using Khedmetak.DAL.Repo.Abstraction;
using Khedmetak.DAL.Repo.Abstraction.UnitOfWork;
using Khedmetak.DAL.Repositories.Interfaces;
using Moq;
using Xunit;

namespace BLL.Test.UserDashboardServiceTest
{
    public class UserDashboardServiceTests
    {
        private readonly Mock<IChatSessionRepository> _chatRepoMock = new();
        private readonly Mock<IUnitOfWork> _unitMock = new();
        private readonly Mock<IUserDocumentRepository> _docRepoMock = new();


        private UserDashboardService CreateService()
        {
            _unitMock
                .Setup(x => x.UserDocuments)
                .Returns(_docRepoMock.Object);


            return new UserDashboardService(
                _chatRepoMock.Object,
                _unitMock.Object);
        }



        [Fact]
        public async Task GetStatsAsync_Should_Return_User_Statistics()
        {
            // Arrange

            var sessions = new List<ChatSession>
            {
                new ChatSession
                {
                    Id = 1,
                    UserId = 1,
                    GovServiceId = 10,
                    Status = RequestStatus.Pending,
                    StartedAt = DateTime.UtcNow
                },

                new ChatSession
                {
                    Id = 2,
                    UserId = 1,
                    GovServiceId = 20,
                    Status = RequestStatus.Completed,
                    StartedAt = DateTime.UtcNow.AddMinutes(-10)
                }
            };


            _chatRepoMock
                .Setup(x => x.GetByUserIdWithDetailsAsync(1))
                .ReturnsAsync(sessions);



            var documents = new List<UserDocument>
            {
                new UserDocument(),
                new UserDocument(),
                new UserDocument()
            };


            _docRepoMock
                .Setup(x => x.GetByUserIdAsync(1))
                .ReturnsAsync(documents);



            var service = CreateService();


            // Act

            var result = await service.GetStatsAsync(1);


            // Assert

            result.Should().NotBeNull();

            result.TotalRequests.Should().Be(2);

            result.PendingCount.Should().Be(1);

            result.CompletedCount.Should().Be(1);

            result.TotalUploadedFiles.Should().Be(3);

            result.TotalChatSessions.Should().Be(2);

            result.RecentRequests.Should()
                .HaveCount(2);
        }




        [Fact]
        public async Task GetMyRequestsAsync_Should_Return_Only_Service_Requests()
        {
            // Arrange

            var sessions = new List<ChatSession>
            {
                new ChatSession
                {
                    Id = 1,
                    GovServiceId = 5,
                    Status = RequestStatus.Pending,
                    StartedAt = DateTime.UtcNow
                },

                new ChatSession
                {
                    Id = 2,
                    GovServiceId = null,
                    Status = RequestStatus.Pending,
                    StartedAt = DateTime.UtcNow
                }
            };


            _chatRepoMock
                .Setup(x => x.GetByUserIdWithDetailsAsync(1))
                .ReturnsAsync(sessions);



            var service = CreateService();



            // Act

            var result =
                await service.GetMyRequestsAsync(1);



            // Assert

            result.Should()
                .HaveCount(1);


            result.First()
                .GovServiceId
                .Should()
                .Be(5);
        }





        [Fact]
        public async Task LinkSessionToServiceAsync_Should_Return_False_When_Session_Not_Found()
        {
            // Arrange

            var sessionGuid = Guid.NewGuid();


            _chatRepoMock
                .Setup(x => x.GetBySessionGuidAsync(sessionGuid))
                .ReturnsAsync((ChatSession)null);



            var dto = new LinkSessionServiceRequestDto
            {
                SessionGuidId = sessionGuid
            };


            var service = CreateService();



            // Act

            var result =
                await service.LinkSessionToServiceAsync(
                    1,
                    dto);



            // Assert

            result.Should()
                .BeFalse();
        }





        [Fact]
        public async Task LinkSessionToServiceAsync_Should_Return_False_When_Owner_Is_Different()
        {
            // Arrange

            var sessionGuid = Guid.NewGuid();


            var session = new ChatSession
            {
                Id = 1,
                UserId = 2,
                SessionGuid = sessionGuid
            };


            _chatRepoMock
                .Setup(x => x.GetBySessionGuidAsync(sessionGuid))
                .ReturnsAsync(session);



            var dto = new LinkSessionServiceRequestDto
            {
                SessionGuidId = sessionGuid
            };


            var service = CreateService();



            // Act

            var result =
                await service.LinkSessionToServiceAsync(
                    1,
                    dto);



            // Assert

            result.Should()
                .BeFalse();
        }





        [Fact]
        public async Task LinkSessionToServiceAsync_Should_Link_Session_Successfully()
        {
            // Arrange

            var sessionGuid = Guid.NewGuid();


            var session = new ChatSession
            {
                Id = 1,
                UserId = null,
                SessionGuid = sessionGuid
            };


            _chatRepoMock
                .Setup(x => x.GetBySessionGuidAsync(sessionGuid))
                .ReturnsAsync(session);



            var dto = new LinkSessionServiceRequestDto
            {
                SessionGuidId = sessionGuid,
                GovServiceId = 10,
                Status = "Completed"
            };



            var service = CreateService();



            // Act

            var result =
                await service.LinkSessionToServiceAsync(
                    5,
                    dto);



            // Assert

            result.Should()
                .BeTrue();


            session.UserId
                .Should()
                .Be(5);


            session.GovServiceId
                .Should()
                .Be(10);


            session.Status
                .Should()
                .Be(RequestStatus.Completed);



            _chatRepoMock.Verify(
                x => x.Update(session),
                Times.Once);



            _unitMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }
    }
}