using AutoMapper;
using FluentAssertions;
using Khedmetak.BLL.DTOS.UserDocument;
using Khedmetak.BLL.Services.Implementation;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.Abstraction;
using Khedmetak.DAL.Repo.Abstraction.UnitOfWork;
using Khedmetak.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace BLL.Test.UserDocumentServiceTest
{
    public class UserDocumentServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitMock = new();
        private readonly Mock<IUserDocumentRepository> _docRepoMock = new();
        private readonly Mock<IChatSessionRepository> _chatRepoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly IConfiguration _configuration;



        public UserDocumentServiceTests()
        {
            var settings = new Dictionary<string, string>
            {
                {
                    "UploadsPath",
                    Path.Combine(Path.GetTempPath(), "uploads")
                }
            };


            _configuration =
                new ConfigurationBuilder()
                .AddInMemoryCollection(settings!)
                .Build();


            _unitMock
                .Setup(x => x.UserDocuments)
                .Returns(_docRepoMock.Object);
        }




        private UserDocumentService CreateService()
        {
            return new UserDocumentService(
                _unitMock.Object,
                _chatRepoMock.Object,
                _mapperMock.Object,
                _configuration);
        }




        [Fact]
        public async Task GetUserDocumentsAsync_Should_Return_Documents()
        {
            // Arrange

            var docs = new List<UserDocument>
            {
                new UserDocument
                {
                    Id = 1,
                    FileName = "test.pdf"
                }
            };


            _docRepoMock
                .Setup(x => x.GetByUserIdAsync(1))
                .ReturnsAsync(docs);



            _mapperMock
                .Setup(x => x.Map<IEnumerable<UserDocumentDto>>(docs))
                .Returns(new List<UserDocumentDto>
                {
                    new UserDocumentDto()
                });



            var service = CreateService();



            // Act

            var result =
                await service.GetUserDocumentsAsync(1);



            // Assert

            result.Should()
                .HaveCount(1);
        }





        [Fact]
        public async Task UploadDocumentAsync_Should_Save_Document()
        {
            // Arrange

            var service = CreateService();


            var stream = new MemoryStream(
                new byte[] { 1, 2, 3 });



            var fileMock = new Mock<IFormFile>();


            fileMock
                .Setup(x => x.FileName)
                .Returns("test.pdf");


            fileMock
                .Setup(x => x.ContentType)
                .Returns("application/pdf");


            fileMock
                .Setup(x => x.CopyToAsync(
                    It.IsAny<Stream>(),
                    default))
                .Returns<Stream, CancellationToken>(
                    (s, token) =>
                    {
                        stream.Position = 0;
                        return stream.CopyToAsync(s);
                    });



            var dto = new UploadDocumentDto
            {
                File = fileMock.Object,
                RequiredDocumentId = 1
            };



            _mapperMock
                .Setup(x => x.Map<UserDocumentDto>(
                    It.IsAny<UserDocument>()))
                .Returns(new UserDocumentDto());



            // Act

            var result =
                await service.UploadDocumentAsync(
                    1,
                    dto);



            // Assert

            result.Should()
                .NotBeNull();



            _docRepoMock.Verify(
                x => x.Add(It.IsAny<UserDocument>()),
                Times.Once);



            _unitMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }







        [Fact]
        public async Task UploadDocumentAsync_Should_Link_Session_When_Guid_Exists()
        {
            // Arrange

            var service = CreateService();


            var guid = Guid.NewGuid();


            var session = new ChatSession
            {
                Id = 10,
                SessionGuid = guid,
                UserId = null
            };


            _chatRepoMock
                .Setup(x => x.GetBySessionGuidAsync(guid))
                .ReturnsAsync(session);



            var fileMock = new Mock<IFormFile>();


            fileMock
                .Setup(x => x.FileName)
                .Returns("image.png");


            fileMock
                .Setup(x => x.ContentType)
                .Returns("image/png");



            fileMock
                .Setup(x => x.CopyToAsync(
                    It.IsAny<Stream>(),
                    default))
                .Returns(Task.CompletedTask);



            var dto = new UploadDocumentDto
            {
                File = fileMock.Object,
                SessionGuidId = guid
            };



            _mapperMock
                .Setup(x => x.Map<UserDocumentDto>(
                    It.IsAny<UserDocument>()))
                .Returns(new UserDocumentDto());



            // Act

            var result =
                await service.UploadDocumentAsync(
                    5,
                    dto);



            // Assert

            session.UserId
                .Should()
                .Be(5);



            _chatRepoMock.Verify(
                x => x.Update(session),
                Times.Once);


            result.Should()
                .NotBeNull();
        }






        [Fact]
        public async Task DeleteDocumentAsync_Should_Delete_Document()
        {
            // Arrange

            var service = CreateService();


            var doc = new UserDocument
            {
                Id = 1,
                UserId = 5,
                FilePath = "uploads/5/test.pdf"
            };


            _docRepoMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(doc);



            // Act

            var result =
                await service.DeleteDocumentAsync(
                    1,
                    5);



            // Assert

            result.Should()
                .BeTrue();



            _docRepoMock.Verify(
                x => x.Delete(doc),
                Times.Once);



            _unitMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }






        [Fact]
        public async Task DeleteDocumentAsync_Should_Return_False_When_Not_Owner()
        {
            // Arrange

            var service = CreateService();


            var doc = new UserDocument
            {
                Id = 1,
                UserId = 10
            };


            _docRepoMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(doc);



            // Act

            var result =
                await service.DeleteDocumentAsync(
                    1,
                    5);



            // Assert

            result.Should()
                .BeFalse();


            _docRepoMock.Verify(
                x => x.Delete(It.IsAny<UserDocument>()),
                Times.Never);
        }
    }
}