using Khedmetak.BLL.DTOS.Admin;
using Khedmetak.BLL.DTOS.GovService;
using Khedmetak.BLL.DTOS.GovServiceDetails;

namespace Khedmetak.BLL.Services.Abstraction
{
    public interface IGovServiceService
    {
        Task<IEnumerable<GovServiceDto>> GetAllServicesAsync();
        Task<IEnumerable<GovServiceDto>> GetServicesByCategoryAsync(int categoryId);
        Task<GovServiceDetailsDto?> GetServiceDetailsAsync(int id);

        Task<GovServiceDto> CreateServiceAsync(CreateGovServiceDto dto);
        Task<GovServiceDto?> UpdateServiceAsync(int id, UpdateGovServiceDto dto);
        Task<bool> DeleteServiceAsync(int id);

        //----------------
        Task<CurrentServiceDetailsDTO?> GetCurrentServiceDetailsAsync(int id);

    }
}
