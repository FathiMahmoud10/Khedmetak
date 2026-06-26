using Qdrant.Client.Grpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.Services.Abstraction
{
    public interface IVectorDBOperationsService
    {
        public Task AddOrUpdateGovServiceToVectorDBAsync(int serviceId);
        //public  Task UpdateGovServiceInVectorDBAsync(int serviceId);
        public Task DeleteGovServiceFromVectorDBAsync(int serviceId);

        public Task<IReadOnlyList<ScoredPoint>> SearchInVectorDBAsync(string userQustion);

    }
}
