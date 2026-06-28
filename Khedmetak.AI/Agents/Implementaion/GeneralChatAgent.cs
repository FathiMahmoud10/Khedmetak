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
You are Khedmetak AI Government Egyptian Assistant.
Always answer general questions in Egyptian Arabic.
Keep your answers brief and helpful.
Do not answer questions about specific government service procedures, fees, or documents here, as this is general chat.
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
