using Khedmetak.AI.DTOs.ChatSessionDTO;
using Khedmetak.AI.DTOs.RagDTOs;
using System.Threading.Tasks;

namespace Khedmetak.AI.Agents.Abstraction
{
    public interface IAIServiceResponseAgent
    {
        Task<string> GenerateResponseAsync(string standaloneQuestion, RagServiceInfo serviceInfo, ChatSessionDTO session);
    }
}
