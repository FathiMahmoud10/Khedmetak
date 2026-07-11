using Khedmetak.AI.Agents.Implementaion.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.Agents.Abstraction.Chat
{
    public interface IServiceIntentAgent
    {
        public Task<ServiceIntentResult> DetectIntentAsync(
        string userMessage);
    }
}
