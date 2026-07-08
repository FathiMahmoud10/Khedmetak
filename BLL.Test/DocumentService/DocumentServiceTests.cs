using AutoMapper;
using FluentAssertions;
using Khedmetak.BLL.DTOS.UploadDocument.Khedmetak.BLL.DTOS.Documents;
using Khedmetak.BLL.DTOS.Documents;
using Khedmetak.BLL.Services.Implementation;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.Abstraction.UnitOfWork;
using Khedmetak.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;

namespace BLL.Test.DocumentServiceTest
{
    public class DocumentServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitMock = new();
        private readonly Mock<IUserDocumentRepository> _documentRepoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<IHostEnvironment> _envMock = new();


        private DocumentService CreateService()
        {
            _envMock
                .Setup(x => x.ContentRootPath)
                .Returns(Path.GetTempPath());


            _unitMock
                .Setup(x => x.UserDocuments)
                .Returns(_documentRepoMock.Object);


            return new DocumentService(
                _unitMock.Object,
                _mapperMock.Object,
                _envMock.Object);
        }



        [Fact]
        public async Task UploadDocumentAsync_Should_Return_False_When_File_Is_Null()
        {
            var service = CreateService();


            var dto = new UploadDocumentDto
            {
                File = null
            };


            var result = await service.UploadDocumentAsync(dto, 1);


            result.Success.Should().BeFalse();
            result.Message.Should()
                .Be("الملف فاضي أو مش موجود");
        }



        [Fact]
        public async Task UploadDocumentAsync_Should_Return_False_When_File_Size_Too_Large()
        {
            var service = CreateService();


            var file = new Mock<IFormFile>();

            file.Setup(x => x.Length)
                .Returns(6 * 1024 * 1024);

            file.Setup(x => x.FileName)
                .Returns("test.pdf");


            var dto = new UploadDocumentDto
            {
                File = file.Object
            };


            var result = await service.UploadDocumentAsync(dto, 1);


            result.Success.Should().BeFalse();

            result.Message.Should()
                .Be("حجم الملف أكبر من 5 MB");
        }





        [Fact]
        public async Task UploadDocumentAsync_Should_Save_File()
        {
            var service = CreateService();


            var stream = new MemoryStream(
                new byte[] { 1, 2, 3 });


            var file = new Mock<IFormFile>();

            file.Setup(x => x.Length)
                .Returns(stream.Length);


            file.Setup(x => x.FileName)
                .Returns("test.pdf");


            file.Setup(x => x.CopyToAsync(
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
                File = file.Object,
                RequiredDocumentId = 1
            };


            _mapperMock
                .Setup(x => x.Map<UserDocumentDto>(
                    It.IsAny<UserDocument>()))
                .Returns(new UserDocumentDto());


            var result =
                await service.UploadDocumentAsync(dto, 1);



            result.Success.Should().BeTrue();


            _documentRepoMock.Verify(
                x => x.Add(It.IsAny<UserDocument>()),
                Times.Once);


            _unitMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }





        [Fact]
        public async Task GetUserDocumentsAsync_Should_Return_Documents()
        {
            var service = CreateService();


            var documents = new List<UserDocument>
            {
                new UserDocument
                {
                    Id = 1,
                    FileName = "test.pdf"
                }
            };


            _documentRepoMock
                .Setup(x => x.GetByUserIdAsync(1))
                .ReturnsAsync(documents);



            _mapperMock
                .Setup(x => x.Map<IEnumerable<UserDocumentDto>>(documents))
                .Returns(new List<UserDocumentDto>
                {
                    new UserDocumentDto()
                });



            var result =
                await service.GetUserDocumentsAsync(1);



            result.Should().HaveCount(1);
        }





        [Fact]
        public async Task SaveUserDocumentsAsync_Should_Return_True()
        {
            var service = CreateService();


            var stream = new MemoryStream(
                new byte[] { 1, 2, 3 });


            var file = new Mock<IFormFile>();

            file.Setup(x => x.Length)
                .Returns(stream.Length);


            file.Setup(x => x.FileName)
                .Returns("test.pdf");



            file.Setup(x => x.CopyToAsync(
                It.IsAny<Stream>(),
                default))
                .Returns<Stream, CancellationToken>(
                (s, token) =>
                {
                    stream.Position = 0;
                    return stream.CopyToAsync(s);
                });



            var result =
                await service.SaveUserDocumentsAsync(
                    new List<IFormFile>
                    {
                        file.Object
                    },
                    1,
                    null);



            result.Should().BeTrue();



            _documentRepoMock.Verify(
                x => x.Add(It.IsAny<UserDocument>()),
                Times.Once);


            _unitMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }





        [Fact]
        public async Task SaveUserDocumentsWithDetailsAsync_Should_Return_List()
        {
            var service = CreateService();


            var stream = new MemoryStream(
                new byte[] { 1, 2, 3 });


            var file = new Mock<IFormFile>();

            file.Setup(x => x.Length)
                .Returns(stream.Length);


            file.Setup(x => x.FileName)
                .Returns("test.pdf");



            file.Setup(x => x.CopyToAsync(
                It.IsAny<Stream>(),
                default))
                .Returns<Stream, CancellationToken>(
                (s, token) =>
                {
                    stream.Position = 0;
                    return stream.CopyToAsync(s);
                });



            _mapperMock
                .Setup(x => x.Map<List<UserDocumentDto>>(
                    It.IsAny<List<UserDocument>>()))
                .Returns(new List<UserDocumentDto>
                {
                    new UserDocumentDto()
                });



            var result =
                await service.SaveUserDocumentsWithDetailsAsync(
                    new List<IFormFile>
                    {
                        file.Object
                    },
                    1,
                    null);



            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }
    }
}