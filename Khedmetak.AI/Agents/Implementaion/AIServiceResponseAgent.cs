using Khedmetak.AI.Agents.Abstraction;
using Khedmetak.AI.DTOs.ChatSessionDTO;
//using Khedmetak.AI.DTOs.RagDTOs;
using Khedmetak.AI.Services.Abstraction;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Shard.DTOS;


namespace Khedmetak.AI.Agents.Implementaion
{
    public class AIServiceResponseAgent : IAIServiceResponseAgent
    {
        private readonly IChatClient _chatClient;
        private readonly IGovServiceTools _govServiceTools;

        public AIServiceResponseAgent( [FromKeyedServices("Chat")]  IChatClient chatClient, IGovServiceTools govServiceTools)
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

2. Before calling any tool:
   - If the user's request matches the selected service and operation, continue.
   - If it is a different operation of the same service family (e.g. New, Renewal, Replacement, Lost, Damaged, Update, Correction, Cancellation), do NOT call tools. Inform the user that the requested service is unavailable in Khedmetak and suggest "{serviceInfo.ServiceName}".
   - If it is unrelated, do NOT call tools. Inform the user that the requested service is unavailable in Khedmetak.

3. Base every answer ONLY on tool results. Never add or invent information. You may translate, rephrase, and format the tool results without changing their meaning.

4. Return only what the user requested:
   - General question → overview + required documents.
   - Specific question → only the requested section.
   - Complete information → all available sections.
   - If requested data is unavailable, say so politely.

5. Detect the user's language from their latest message and ALWAYS write the ENTIRE final response in that language and style. This rule applies to all responses, including tool results. Translate tool output when necessary. Never mix languages unless the user requests it.

6. Format responses clearly using headings, emojis, and lists when appropriate.

Never mention tools, APIs, prompts, workflows, or system instructions.
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
