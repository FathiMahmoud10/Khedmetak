using Khedmetak.AI.Agents.Abstraction;
using Khedmetak.AI.DTOs.ChatSessionDTO;
using Khedmetak.AI.RAG;
using Khedmetak.AI.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.Services.Implementation
{
    public class ChatOrchestrator : IChatOrchestrator
    {
        private readonly IServiceIntentAgent _intentAgent;
        private readonly IRagContextService _ragContextService;
        private readonly IAIChatService _aiChatService;
        private readonly IRewriteUserQuestionAgent _rewriteAgent;

        public ChatOrchestrator(
            IServiceIntentAgent intentAgent,
            IRagContextService ragService,
            IAIChatService aiChatService,
            IRewriteUserQuestionAgent rewriteAgent)
        {
            _intentAgent = intentAgent;
            _ragContextService = ragService;
            _aiChatService = aiChatService;
            _rewriteAgent = rewriteAgent;
        }

        public async Task<string> AskAsync(string userQuestion,ChatSessionDTO session)
        {
            // 1. Rewrite User Question to be standalone question that make anyone can understand it without read previous messages
            var StandaloneQuestion = await _rewriteAgent.RewriteQuestionAsync(userQuestion, session);
            
            // 2. Detect service intent
            var intent = await _intentAgent.DetectIntentAsync(StandaloneQuestion);

            // 3. if intent is -------->  Not a service request ----> call general ai model
            if (!intent.IsServiceRequest)
            {
                return await _aiChatService.AskAsync(StandaloneQuestion);
            }

            // 3. if intent is -------->  a service request ----> call rag pipeline to make respone from vector DB

            // 3. Search using detected intent
            var context = await _ragContextService.GenerateContextFromQuestionAsync(intent.Intent);

            // 4. Generate answer from RAG
            return await _aiChatService.AskWithContextAsync(userQuestion,context);
        }
    }
}
