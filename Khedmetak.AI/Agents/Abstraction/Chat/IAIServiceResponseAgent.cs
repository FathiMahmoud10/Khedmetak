using Shard.DTOS;

namespace Khedmetak.AI.Agents.Abstraction.Chat
{
    public interface IAIServiceResponseAgent
    {
        Task<string> GenerateResponseAsync(string standaloneQuestion, RagServiceInfo serviceInfo);
    }
}
