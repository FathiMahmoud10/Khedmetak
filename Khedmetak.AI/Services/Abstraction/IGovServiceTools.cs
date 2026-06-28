using Khedmetak.AI.DTOs.AIAgentToolsDtos;
using Khedmetak.BLL.DTOS.GovServiceDetails;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Khedmetak.AI.Services.Abstraction
{
    public interface IGovServiceTools
    {
        Task<GovServiceSummaryDto?> GetServiceSummary(int serviceId);
        Task<List<RequiredDocumentDto>?> GetRequiredDocuments(int serviceId);
        Task<List<ServiceStepDto>?> GetServiceSteps(int serviceId);
        Task<GovServiceFeesDto?> GetServiceFees(int serviceId);
        Task<string?> GetServiceEstimatedTime(int serviceId);
        //Task<List<ServiceOptionDto>?> GetServiceOptions(int serviceId);
        //Task<List<ServiceGeneralDocDto>?> GetGeneralDocuments(int serviceId);
    }
}
