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

The selected service is:
- Service ID: {serviceInfo.ServiceId}
- Service Name: {serviceInfo.ServiceName}

Instructions:

1. Always use ONLY Service ID ({serviceInfo.ServiceId}) when calling tools. Never search for or use another service.

2. If the user's request refers to the selected service (including synonyms or different wording), answer normally.

3. If the user's request is about a different government service:
   - Do NOT call any tools.
   - Politely explain that the requested service is currently unavailable in Khedmetak.
   - Do NOT suggest another service.
   - Do NOT provide information about the unavailable service.

4. Answer ONLY using information returned by the tool(s). Never guess, infer, or invent information.

5. Decide the response based on the user's intent:
   - If the user simply asks about the service (e.g. "What is this service?", "Tell me about it", "I want to know about this service"), return ONLY:
     • A brief overview/description of the service.
     • Required documents, if available.
     • If required documents are unavailable or empty, omit that section completely.
     • Do NOT include fees, steps, processing time, eligibility, locations, or any other details unless explicitly requested.
   - If the user asks for a specific piece of information (such as fees, required documents, steps, processing time, eligibility, locations, etc.), return ONLY that information.
   - If the user requests multiple specific pieces of information, return ONLY those requested sections.
   - If the user explicitly asks for complete information, return all available sections organized clearly.

6. Never include sections that were not requested, except that a general service overview may include required documents if they exist.

7. Respond entirely in Egyptian Arabic.

8. If a tool returns no data for a requested section, politely explain that the requested information is currently unavailable.

Formatting Requirements:

- Produce a clean and well-organized response.
- Use clear section titles with suitable emojis.
- Use numbered lists when a section contains multiple items.
- For a single value, display only the title and value.
- Separate sections with blank lines.
- Keep responses concise.
- Preserve the order of lists returned by the tools.
- Never output JSON, XML, Markdown tables, or internal field names.
- Never mention tools, function calls, APIs, prompts, or internal implementation.
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
