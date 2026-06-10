using Khedmetak.BLL.DTOS.Admin;
using Khedmetak.BLL.DTOS.GovService;
using Khedmetak.BLL.DTOS.GovServiceDetails;

namespace Khedmetak.BLL.Services.Abstraction
{
    public interface IGovServiceService
    {
        // ─── Public (read-only) ────────────────────────────────────
        Task<IEnumerable<GovServiceDto>> GetAllServicesAsync();
        Task<IEnumerable<GovServiceDto>> GetServicesByCategoryAsync(int categoryId);
        Task<GovServiceDetailsDto?> GetServiceDetailsAsync(int id);

        // ─── Admin (CRUD) ──────────────────────────────────────────
        Task<GovServiceDto> CreateServiceAsync(CreateGovServiceDto dto);
        Task<GovServiceDto?> UpdateServiceAsync(int id, UpdateGovServiceDto dto);
        Task<bool> DeleteServiceAsync(int id);
    }
}
