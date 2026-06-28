using Khedmetak.AI.DTOs.RagDTOs;
using System.Threading.Tasks;

namespace Khedmetak.AI.RAG
{
    public interface IRagService
    {
        Task<RagServiceInfo?> SearchServiceAsync(string standaloneQuestion);
    }
}
