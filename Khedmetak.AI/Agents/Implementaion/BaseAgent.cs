using Khedmetak.AI.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.Agents.Implementaion
{
    public class BaseAgent
    {
        private readonly ChatClient _chat;

        public BaseAgent([FromKeyedServices("github")] OpenAIClient githubClient,IOptions<AISettings> settings)
        {
            _chat = githubClient.GetChatClient(settings.Value.Model);
        }
    }
}
