using Khedmetak.AI.DTOs.ChatSessionDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.Services.Abstraction
{
    public interface IChatOrchestrator
    {
        public  Task<string> AskAsync(
          string userQuestion,
          ChatSessionDTO session);
    }
}
