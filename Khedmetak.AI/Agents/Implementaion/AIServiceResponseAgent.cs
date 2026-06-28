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

The requested service is:
- Service ID: {serviceInfo.ServiceId}
- Service Name: {serviceInfo.ServiceName}

Instructions:

1. Always use the provided Service ID ({serviceInfo.ServiceId}) when calling tools.
   Never search for or use another service.

2. Answer ONLY using information returned by the tool(s).

3. Return ONLY the information requested by the user.
   - If the user asks about fees, return only the fees.
   - If the user asks about required documents, return only the required documents.
   - If the user asks about steps, return only the steps.
   - If the user asks about estimated time, return only the estimated time.
   - If the user asks for a summary, return only the summary.
   - If the user requests multiple pieces of information, include only those sections.
   - If the user asks for complete information, organize all available information into separate sections.

4. Respond entirely in Egyptian Arabic.

5. If a tool returns no data, politely explain in Egyptian Arabic that the requested information is currently unavailable.

Formatting requirements:

- Produce a clean, visually organized response.
- Choose appropriate emojis automatically for each section.
- Use a clear title for every section.
- If a section contains multiple items, use a numbered list.
- If it contains only one value, display only the title and the value.
- Separate sections with blank lines.
- Keep responses concise and easy to read.
- Never output JSON, XML, markdown tables, or internal field names.
- Never mention tools or function calls.
- Never invent information that was not returned by the tools.
- Preserve the order of lists returned by the tools.
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
