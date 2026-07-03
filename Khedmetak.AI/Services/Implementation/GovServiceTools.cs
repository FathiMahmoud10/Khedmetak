using Khedmetak.AI.DTOs.AIAgentToolsDtos;
using Khedmetak.AI.Services.Abstraction;
using Khedmetak.BLL.DTOS.GovServiceDetails;
using Khedmetak.BLL.Services.Abstraction;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Khedmetak.AI.Services.Implementation
{
    public class GovServiceTools : IGovServiceTools
    {
        private readonly IGovServiceService _govServiceService;

        public GovServiceTools(IGovServiceService govServiceService)
        {
            _govServiceService = govServiceService;
        }

        [Description("Get a list of all available government services.")]
        public async Task<List<ServiceDTO>> GetAllServices()
        {
            var services = await _govServiceService.GetAllServicesAsync();
            var serviceDTOs = new List<ServiceDTO>();   
            foreach (var service in services)
            {
                serviceDTOs.Add(new ServiceDTO
                {
                    ServiceName = service.SrvName,
                    Description = service.SrvDesc
                });
            }
            return serviceDTOs;
        }

        [Description("Get summary and basic description of the government service.")]
        public async Task<GovServiceSummaryDto?> GetServiceSummary(
            [Description("The ID of the government service.")] int serviceId)
        {
            var service = await _govServiceService.GetServiceDetailsAsync(serviceId);
            if (service == null) return null;
            return new GovServiceSummaryDto
            {
                Id = service.Id,
                ServiceName = service.SrvName,
                Description = service.SrvDesc
            };
        }

        [Description("Get the required documents needed to apply for the government service.")]
        public async Task<List<RequiredDocumentDto>?> GetRequiredDocuments(
            [Description("The ID of the government service.")] int serviceId)
        {
            var service = await _govServiceService.GetServiceDetailsAsync(serviceId);
            return service?.RequiredDocuments;
        }

        [Description("Get the steps or procedures to execute the government service.")]
        public async Task<List<ServiceStepDto>?> GetServiceSteps(
            [Description("The ID of the government service.")] int serviceId)
        {
            var service = await _govServiceService.GetServiceDetailsAsync(serviceId);
            return service?.Steps;
        }

        [Description("Get the fees associated with the government service.")]
        public async Task<GovServiceFeesDto?> GetServiceFees(
            [Description("The ID of the government service.")] int serviceId)
        {
            var service = await _govServiceService.GetServiceDetailsAsync(serviceId);
            if (service == null) return null;
            return new GovServiceFeesDto
            {
                BaseFees = service.SrvFees,
                EstimatedFees = service.EstimatedFees
            };
        }

        [Description("Get the estimated time required to complete the government service.")]
        public async Task<string?> GetServiceEstimatedTime(
            [Description("The ID of the government service.")] int serviceId)
        {
            var service = await _govServiceService.GetServiceDetailsAsync(serviceId);
            return service?.SrvTime;
        }

        [Description("Get options or alternatives available for the government service.")]
        public async Task<List<ServiceOptionDto>?> GetServiceOptions(
            [Description("The ID of the government service.")] int serviceId)
        {
            var service = await _govServiceService.GetServiceDetailsAsync(serviceId);
            return service?.Options;
        }

        // [Description("Get general documents or reference material for the government service.")]
        // public async Task<List<ServiceGeneralDocDto>?> GetGeneralDocuments(
        //     [Description("The ID of the government service.")] int serviceId)
        // {
        //     var service = await _govServiceService.GetServiceDetailsAsync(serviceId);
        //     return service?.GeneralDocs;
        // }
    }
}
