// Khedmetak.AI.Agents.Abstraction/IRelevanceValidatorAgent.cs
//using Khedmetak.AI.DTOs.RagDTOs;
using Shard.DTOS;
using Khedmetak.AI.RAG;

namespace Khedmetak.AI.Agents.Abstraction
{
    public interface IRelevanceValidatorAgent
    {
        Task<bool> IsRelevantAsync(string userQuestion, RagServiceInfo serviceInfo);
    }
}