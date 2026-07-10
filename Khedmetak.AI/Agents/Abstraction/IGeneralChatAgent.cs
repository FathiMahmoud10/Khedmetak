using Khedmetak.AI.DTOs.ChatSessionDTO;
using System.Threading.Tasks;

namespace Khedmetak.AI.Agents.Abstraction
{
    public interface IGeneralChatAgent
    {
        Task<string> AnswerAsync(string standaloneQuestion);
    }
}
