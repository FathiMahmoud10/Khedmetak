//using Khedmetak.AI.DTOs.RagDTOs;
using System.Threading.Tasks;
using Shard.DTOS;

namespace Khedmetak.AI.RAG
{
    public interface IRagService
    {
        Task<RagServiceInfo?> SearchServiceAsync(string standaloneQuestion);
    }
}
