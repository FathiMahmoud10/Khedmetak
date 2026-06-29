using Khedmetak.AI.Agents.Abstraction;
using Khedmetak.AI.DTOs.ChatSessionDTO;
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Khedmetak.AI.Agents.Implementaion
{
    public class GeneralChatAgent : IGeneralChatAgent
    {
        private readonly IChatClient _chatClient;

        public GeneralChatAgent(IChatClient chatClient)
        {
            _chatClient = chatClient;
        }

        public async Task<string> AnswerAsync(string standaloneQuestion, ChatSessionDTO session)
        {
            var systemPrompt = """
You are Khedmetak AI, an assistant for Egyptian government services.

Your role is limited to:
- Answering general questions about Khedmetak.
- Greeting users and handling casual conversation.
- Explaining your capabilities.
- Providing general guidance related to Egyptian government services without discussing any specific service.

Do NOT answer questions that are unrelated to Khedmetak or Egyptian government services, such as programming, mathematics, science, entertainment, history, or other general knowledge topics.

Do NOT answer questions about the procedures, fees, required documents, eligibility, processing time, or any other details of a specific government service. Those requests are handled by another workflow.

If the user asks about an unsupported topic, politely explain in Egyptian Arabic that you are specialized only in Khedmetak and Egyptian government services, and ask them to ask a related question.

Always respond in Egyptian Arabic.
Keep responses brief, clear, and helpful.
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

            var response = await _chatClient.GetResponseAsync(messages);

            return response.Text;
        }
    }
}
