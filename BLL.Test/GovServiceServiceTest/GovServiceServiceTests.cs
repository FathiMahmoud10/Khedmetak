using AutoMapper;
using FluentAssertions;
using Khedmetak.BLL.DTOS.Admin;
using Khedmetak.BLL.DTOS.GovService;
using Khedmetak.BLL.DTOS.GovServiceDetails;
using Khedmetak.BLL.Services.Implementation;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repositories.Interfaces;
using Khedmetak.DAL.Repo.Abstraction.UnitOfWork;
using Moq;
using Xunit;

namespace BLL.Test.GovServiceServiceTest
{
    public class GovServiceServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitMock = new();
        private readonly Mock<IGovServiceRepository> _repoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();


        private GovServiceService CreateService()
        {
            _unitMock
                .Setup(x => x.GovServices)
                .Returns(_repoMock.Object);


            return new GovServiceService(
                _unitMock.Object,
                _mapperMock.Object);
        }



        [Fact]
        public async Task GetAllServicesAsync_Should_Return_Services()
        {
            var service = CreateService();


            var services = new List<GovService>
            {
                new GovService
                {
                    Id = 1,
                    SrvName = "Service 1"
                }
            };


            _repoMock
                .Setup(x => x.GetAllWithCategoryAsync())
                .ReturnsAsync(services);


            _mapperMock
                .Setup(x => x.Map<IEnumerable<GovServiceDto>>(services))
                .Returns(new List<GovServiceDto>
                {
                    new GovServiceDto()
                });



            var result = await service.GetAllServicesAsync();


            result.Should().HaveCount(1);
        }




        [Fact]
        public async Task GetServicesByCategoryAsync_Should_Return_Services()
        {
            var service = CreateService();


            var services = new List<GovService>
            {
                new GovService()
            };


            _repoMock
                .Setup(x => x.GetByCategoryAsync(1))
                .ReturnsAsync(services);



            _mapperMock
                .Setup(x => x.Map<IEnumerable<GovServiceDto>>(services))
                .Returns(new List<GovServiceDto>
                {
                    new GovServiceDto()
                });



            var result =
                await service.GetServicesByCategoryAsync(1);


            result.Should().HaveCount(1);
        }





        [Fact]
        public async Task GetServiceDetailsAsync_Should_Return_Details()
        {
            var service = CreateService();


            var entity = new GovService
            {
                Id = 1
            };


            _repoMock
                .Setup(x => x.GetServiceWithDetailsAsync(1))
                .ReturnsAsync(entity);



            _mapperMock
                .Setup(x => x.Map<GovServiceDetailsDto>(entity))
                .Returns(new GovServiceDetailsDto());



            var result =
                await service.GetServiceDetailsAsync(1);


            result.Should().NotBeNull();
        }





        [Fact]
        public async Task GetServiceDetailsAsync_Should_Return_Null_When_NotFound()
        {
            var service = CreateService();


            _repoMock
                .Setup(x => x.GetServiceWithDetailsAsync(1))
                .ReturnsAsync((GovService)null);



            var result =
                await service.GetServiceDetailsAsync(1);


            result.Should().BeNull();
        }





        [Fact]
        public async Task CreateServiceAsync_Should_Create_Service()
        {
            var service = CreateService();


            var dto = new CreateGovServiceDto();


            var entity = new GovService
            {
                Id = 1
            };


            _mapperMock
                .Setup(x => x.Map<GovService>(dto))
                .Returns(entity);



            _repoMock
                .Setup(x => x.GetServiceWithDetailsAsync(1))
                .ReturnsAsync(entity);



            _mapperMock
                .Setup(x => x.Map<GovServiceDto>(entity))
                .Returns(new GovServiceDto());



            var result =
                await service.CreateServiceAsync(dto);



            result.Should().NotBeNull();


            _repoMock.Verify(
                x => x.Add(entity),
                Times.Once);


            _unitMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }





        [Fact]
        public async Task UpdateServiceAsync_Should_Return_Null_When_NotFound()
        {
            var service = CreateService();


            _repoMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((GovService)null);



            var result =
                await service.UpdateServiceAsync(
                    1,
                    new UpdateGovServiceDto());



            result.Should().BeNull();
        }





        [Fact]
        public async Task DeleteServiceAsync_Should_Return_True()
        {
            var service = CreateService();


            var entity = new GovService
            {
                Id = 1
            };


            _repoMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(entity);



            var result =
                await service.DeleteServiceAsync(1);



            result.Should().BeTrue();



            _repoMock.Verify(
                x => x.Delete(entity),
                Times.Once);


            _unitMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }





        [Fact]
        public async Task DeleteServiceAsync_Should_Return_False_When_NotFound()
        {
            var service = CreateService();


            _repoMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((GovService)null);



            var result =
                await service.DeleteServiceAsync(1);



            result.Should().BeFalse();
        }
    }
}