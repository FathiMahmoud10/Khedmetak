using Shard.DTOS;

namespace Khedmetak.AI.Agents.Abstraction
{
    public interface IAIServiceResponseAgent
    {
        Task<string> GenerateResponseAsync(string standaloneQuestion, RagServiceInfo serviceInfo);
    }
}
