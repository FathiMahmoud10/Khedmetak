using Khedmetak.BLL.DTOS.GovService;
using Khedmetak.BLL.DTOS.GovServiceDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.BLL.Services.Abstraction
{
    public interface IGovServiceService
    {
        Task<IEnumerable<GovServiceDto>> GetAllServicesAsync();
        Task<IEnumerable<GovServiceDto>> GetServicesByCategoryAsync(int categoryId);
        Task<GovServiceDetailsDto?> GetServiceDetailsAsync(int id);
    }
}
