using Khedmetak.AI.DTOs.ChatSessionDTO;
using System.Threading.Tasks;

namespace Khedmetak.AI.Agents.Abstraction.Chat
{
    public interface IRewriteQuestionAgent
    {
        Task<string> RewriteQuestionAsync(string userQuestion, ChatSessionDTO? chatSessionDto);
    }
}
