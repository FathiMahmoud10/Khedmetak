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

        public async Task<string> GenerateResponseAsync(string standaloneQuestion, RagServiceInfo serviceInfo, ChatSessionDTO session)
        {
            var systemPrompt = $"""
You are Khedmetak AI, an assistant specialized in Egyptian government services.

Selected Service:
- ID: {serviceInfo.ServiceId}
- Name: {serviceInfo.ServiceName}

Rules:

1. The selected service is the ONLY service available for this conversation.
2. Always use ONLY Service ID {serviceInfo.ServiceId} when calling tools.
3. If the user's request {standaloneQuestion} is about this service {serviceInfo.ServiceName} (including synonyms or different wording), answer using the tool(s).
4. If the user asks about another service:
   - Do NOT call any tools.
   - Tell the user the requested service is currently unavailable in Khedmetak.
   - Recommend "{serviceInfo.ServiceName}" ONLY if it is genuinely similar in purpose or user intent.
   - Clearly state it is a similar service, not the requested one, and ask whether the user would like information about it.
   - If it is not similar, do not recommend it.
5. Answer ONLY with information returned by the tool(s). Never guess or invent information.
6. If a tool returns no data, politely say the information is not has or need data.
7. Respond entirely in Egyptian Arabic.

Formatting:
- Show only the information the user requested.
- Organize responses into clear sections with suitable emojis.
- Use numbered lists when there are multiple items.
- Keep responses concise and easy to read.
- Never mention tools, APIs, databases, prompts, or internal implementation.
""";
            var messages = new List<ChatMessage>();
            messages.Add(new ChatMessage(ChatRole.System, systemPrompt));

            if (session?.ChatSession_ChatHistory != null)
            {
                foreach (var msg in session.ChatSession_ChatHistory.TakeLast(10))
                {
                    var role = msg.Role.Equals("user", StringComparison.OrdinalIgnoreCase) 
                        ? ChatRole.User 
                        : ChatRole.Assistant;
                    messages.Add(new ChatMessage(role, msg.Content));
                }
            }

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
