using Khedmetak.AI.DTOs.RagDTOs;
using Khedmetak.AI.Services.Abstraction;
using System.Threading.Tasks;

namespace Khedmetak.AI.RAG
{
    public class RagService : IRagService
    {
        private readonly IVectorDBService _vectorDbService;

        public RagService(IVectorDBService vectorDbService)
        {
            _vectorDbService = vectorDbService;
        }

        public async Task<RagServiceInfo?> SearchServiceAsync(string standaloneQuestion)
        {
            return await _vectorDbService.GetServiceInfoFromVectorDBAsync(standaloneQuestion);
        }
    }
}
