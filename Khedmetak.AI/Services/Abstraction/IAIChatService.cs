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
        
        //public  Task<string> AskAsync(string newUserMessage);
        public  Task<string> AskAsync(string newUserMessage);
        //public  Task<string> AskWithContextAsync(string userQuestion,string context);
        public  Task<string> RewriteQuestionAsync(string userQuestion,ChatSessionDTO? chatSessionDto);
        public Task<string> AskWithContextAsync(string userQuestion, string context);

    }
}
