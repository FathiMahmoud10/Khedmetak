using Khedmetak.AI.Agents.Abstraction;
using Khedmetak.AI.DTOs.ChatSessionDTO;
using Khedmetak.AI.RAG;
using Khedmetak.AI.Services.Abstraction;
using Khedmetak.BLL.DTOS.GovService;
using Khedmetak.BLL.Services.Abstraction;
using System;
using System.Threading.Tasks;

namespace Khedmetak.AI.Services.Implementation
{
    

    public class AIResponseDTO
    {
       public CurrentServiceDetailsDTO? CurrentServiceDetails { get; set; }
       public string response { get; set; }
    }
    public class ChatOrchestrator : IChatOrchestrator
    {
        private readonly IRewriteQuestionAgent _rewriteAgent;
        private readonly IServiceIntentAgent _intentAgent;
        private readonly IGeneralChatAgent _generalChatAgent;
        private readonly IRagService _ragContextService;
        private readonly IAIServiceResponseAgent _aiResponseAgent;
        private readonly IGovServiceService govService;
        private readonly IRelevanceValidatorAgent _relevanceValidatorAgent;

        private CurrentServiceDetailsDTO currentServiceDetails;
        private bool isServiceChanged = false;
        private int serviceId = 0;

        public ChatOrchestrator(
            IRewriteQuestionAgent rewriteAgent,
            IServiceIntentAgent intentAgent,
            IGeneralChatAgent generalChatAgent,
            IRagService ragContextService,
            IAIServiceResponseAgent aiResponseAgent,
            IGovServiceService govService,
            IRelevanceValidatorAgent relevanceValidatorAgent)
        {
            _rewriteAgent = rewriteAgent;
            _intentAgent = intentAgent;
            _generalChatAgent = generalChatAgent;
            _ragContextService = ragContextService;
            _aiResponseAgent = aiResponseAgent;
            this.govService = govService;
            _relevanceValidatorAgent = relevanceValidatorAgent;

        }

        public async Task<AIResponseDTO> AskAsync(string userQuestion, ChatSessionDTO session)
        {
            // 1. Rewrite User Question to be standalone
            var standaloneQuestion = await _rewriteAgent.RewriteQuestionAsync(userQuestion, session);

            // 2. Detect service intent
            var intent = await _intentAgent.DetectIntentAsync(standaloneQuestion);

            // 3. If intent is NOT a service request, call general AI chat agent
            if (!intent.IsServiceRequest)
            {
                return new AIResponseDTO()
                {
                    CurrentServiceDetails = new CurrentServiceDetailsDTO(),
                    response = await _generalChatAgent.AnswerAsync(standaloneQuestion, session)
                };
            }

            // 4. Retrieve service info from RAG
            var serviceInfo = await _ragContextService.SearchServiceAsync(intent.Intent);

            // 5. If service not found, ask user for clarification
            if (serviceInfo == null)
            {
                
                return new AIResponseDTO()
                {
                    CurrentServiceDetails = new CurrentServiceDetailsDTO()
                    {
                        ServiceName = "خدمة ليست متوفرة"
                    },
                    response = "هذه الخدمة لبست متوفرة حاليا"


                };
            }
            // ✅ NEW: Validate that retrieved service actually matches the question
            var isRelevant = await _relevanceValidatorAgent.IsRelevantAsync(standaloneQuestion, serviceInfo);
            if (!isRelevant)
                return new AIResponseDTO()
                {
                    CurrentServiceDetails = new CurrentServiceDetailsDTO() { ServiceName = "خدمة ليست متوفرة" },
                     response = await _aiResponseAgent.GenerateResponseAsync(standaloneQuestion, serviceInfo)
                    
                };

            if (serviceId != serviceInfo.ServiceId)
            {
                 currentServiceDetails = await govService.GetCurrentServiceDetailsAsync(serviceInfo.ServiceId);
                serviceId = serviceInfo.ServiceId;
            }
            // 6. Generate answer using service response agent (which coordinates function calling)
            return new AIResponseDTO()
            {
                CurrentServiceDetails = currentServiceDetails,
                response = await _aiResponseAgent.GenerateResponseAsync(standaloneQuestion, serviceInfo)

            };
        }
    }
}
