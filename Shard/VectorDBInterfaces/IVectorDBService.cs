using Shard.DTOS;
using System.Threading.Tasks;

namespace Shard.VectorDBInterfaces
{
    public interface IVectorDBService
    {
        Task AddOrUpdateGovServiceToVectorDBAsync(int serviceId);
        Task DeleteGovServiceFromVectorDBAsync(int serviceId);

        Task<RagServiceInfo?> GetServiceInfoFromVectorDBAsync(string userQuestion);
    }
}
