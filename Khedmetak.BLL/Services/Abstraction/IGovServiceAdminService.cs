using Khedmetak.BLL.DTOS.Admin;
using Khedmetak.BLL.DTOS.GovService;

namespace Khedmetak.BLL.Services.Abstraction
{
    public interface IGovServiceAdminService
    {

        Task<ImportServicesResultDto> ImportServicesFromExcelAsync(Stream excelFileStream);

        Task<GovServiceDto> CreateServiceAsync(CreateGovServiceDto dto);
        Task<GovServiceDto?> UpdateServiceAsync(int id, UpdateGovServiceDto dto);
        Task<bool> DeleteServiceAsync(int id);

        Task<GovServiceDto?> UpdateFeesAsync(int id, UpdateFeesDto dto);

        Task<IEnumerable<ServiceStepAdminDto>> GetStepsAsync(int govServiceId);
        Task<ServiceStepAdminDto?> AddStepAsync(int govServiceId, CreateServiceStepDto dto);
        Task<ServiceStepAdminDto?> UpdateStepAsync(int govServiceId, int stepId, UpdateServiceStepDto dto);
        Task<bool> DeleteStepAsync(int govServiceId, int stepId);

        Task<IEnumerable<RequiredDocumentAdminDto>> GetRequiredDocumentsAsync(int govServiceId);
        Task<RequiredDocumentAdminDto?> AddRequiredDocumentAsync(int govServiceId, CreateRequiredDocumentDto dto);
        Task<RequiredDocumentAdminDto?> UpdateRequiredDocumentAsync(int govServiceId, int docId, UpdateRequiredDocumentDto dto);
        Task<bool> DeleteRequiredDocumentAsync(int govServiceId, int docId);
    }
}
