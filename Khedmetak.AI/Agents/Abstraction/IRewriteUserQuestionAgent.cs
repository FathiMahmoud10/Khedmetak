using Khedmetak.AI.DTOs.ChatSessionDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.Agents.Abstraction
{
    public interface IRewriteUserQuestionAgent
    {
        public Task<string> RewriteQuestionAsync(string userQuestion, ChatSessionDTO? chatSessionDto);

    }
}
