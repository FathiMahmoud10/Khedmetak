using Khedmetak.AI.DTOs;
using Khedmetak.AI.DTOs.ChatSessionDTO;
using Khedmetak.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.Services.Abstraction
{
    public interface IAIChatService
    {
        //public Task<string> AskAsync(string newUserMessage,ChatSessionDTO chatSessionMessages);

        public  Task<string> AskAsync(string newUserMessage);
        public  Task<string> AskAsync(string newUserMessage, ChatSessionDTO chatSessionDto);

    }
}
