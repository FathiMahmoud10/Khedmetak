using Khedmetak.AI.Agents.Abstraction;
using Khedmetak.AI.DTOs.ChatSessionDTO;
using Khedmetak.AI.DTOs.RagDTOs;
using Khedmetak.AI.Services.Abstraction;
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Khedmetak.AI.Agents.Implementaion
{
    public class AIServiceResponseAgent : IAIServiceResponseAgent
    {
        private readonly IChatClient _chatClient;
        private readonly IGovServiceTools _govServiceTools;

        public AIServiceResponseAgent(IChatClient chatClient, IGovServiceTools govServiceTools)
        {
            _chatClient = chatClient;
            _govServiceTools = govServiceTools;
        }

        public async Task<string> GenerateResponseAsync(string standaloneQuestion, RagServiceInfo serviceInfo)
        {
            var systemPrompt = $"""
You are Khedmetak AI, an assistant for Egyptian government services.

Selected service:
- Service ID: {serviceInfo.ServiceId}
- Service Name: {serviceInfo.ServiceName}

Rules:

1. Use ONLY Service ID ({serviceInfo.ServiceId}) when calling tools.

2. Before using any tool, compare the user's request with the selected service.
   - If they refer to the same service (same meaning and operation, even with different wording), continue normally.
  - If the user's requested service is different from the selected service but they belong to the same service family or have a similar purpose (e.g. New, Renewal, Replacement, Lost, Damaged, Update, Correction, Cancellation), do NOT call any tools. Inform the user that the exact requested service is currently unavailable in Khedmetak, and suggest the available service:"{serviceInfo.ServiceName}".
   - If they are completely unrelated, do NOT call any tools. Inform the user that the requested service is currently unavailable in Khedmetak.

3. Answer ONLY using tool results. Never guess or invent information.

4. Return only the information the user requests:
   - General question → overview + required documents (if available).
   - Specific question → only the requested section.
   - Complete information → all available sections.

5. Respond entirely in Egyptian Arabic.

6. If a requested section has no data, politely say it is currently unavailable.

Format the response clearly with section titles, emojis, and numbered lists where appropriate. Never mention tools, APIs, or internal implementation.
""";
            var messages = new List<ChatMessage>();
            messages.Add(new ChatMessage(ChatRole.System, systemPrompt));

            //if (session?.ChatSession_ChatHistory != null)
            //{
            //    foreach (var msg in session.ChatSession_ChatHistory.TakeLast(10))
            //    {
            //        var role = msg.Role.Equals("user", StringComparison.OrdinalIgnoreCase) 
            //            ? ChatRole.User 
            //            : ChatRole.Assistant;
            //        messages.Add(new ChatMessage(role, msg.Content));
            //    }
            //}

            messages.Add(new ChatMessage(ChatRole.User, standaloneQuestion));

            var options = new ChatOptions
            {
                Tools =
                [
                    AIFunctionFactory.Create(_govServiceTools.GetServiceSummary),
                    AIFunctionFactory.Create(_govServiceTools.GetRequiredDocuments),
                    AIFunctionFactory.Create(_govServiceTools.GetServiceSteps),
                    AIFunctionFactory.Create(_govServiceTools.GetServiceFees),
                    AIFunctionFactory.Create(_govServiceTools.GetServiceEstimatedTime)
                    //AIFunctionFactory.Create(_govServiceTools.GetServiceOptions, "GetServiceOptions"),
                    //AIFunctionFactory.Create(_govServiceTools.GetGeneralDocuments, "GetGeneralDocuments")
                ]
            };

            var response = await _chatClient.GetResponseAsync(messages,options);

            return response.Text;
        }
    }
}
