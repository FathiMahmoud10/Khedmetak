using Khedmetak.AI.DTOs.RagDTOs;
using System.Threading.Tasks;

namespace Khedmetak.AI.Services.Abstraction
{
    public interface IVectorDBService
    {
        Task AddOrUpdateGovServiceToVectorDBAsync(int serviceId);
        Task DeleteGovServiceFromVectorDBAsync(int serviceId);

        //-------- to get ServiceId and ServiceName  information from Vector DB
        Task<RagServiceInfo?> GetServiceInfoFromVectorDBAsync(string userQuestion);
    }
}
