using Khedmetak.AI.Agents.Abstraction;
using Khedmetak.AI.Configuration;
using Khedmetak.AI.DTOs.ChatSessionDTO;
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
    public class RewriteQuestionAgent : IRewriteQuestionAgent
    {
        private readonly ChatClient _chat;

        public RewriteQuestionAgent([FromKeyedServices("github")] OpenAIClient githubClient, IOptions<AISettings> settings)
        {
            _chat = githubClient.GetChatClient(settings.Value.Model);
        }

        public async Task<string> RewriteQuestionAsync(string userQuestion, ChatSessionDTO? chatSessionDto)
        {
            List<ChatMessage> messages = new();

            messages.Add(ChatMessage.CreateSystemMessage(
               """
You are a question rewriting assistant.

Your task is to determine whether the user's latest message depends on the previous conversation.

Rules:

1. If the user's message is already a complete, standalone question that can be understood without the conversation history, return it EXACTLY as written.
2. Rewrite the user's message ONLY if it depends on previous messages (for example: pronouns like "it", "that", "those", "them", follow-up questions such as "What about the documents?", "How much does it cost?", "Can I renew it online?", etc.).
3. Preserve the user's original intent. Do not answer the question.
4. Do not add information that is not implied by the conversation.
5. Return ONLY the final question. Do not explain your reasoning. Do not use quotation marks or markdown.
6. Reply in English.

Examples:

Conversation:
User: I want to renew my passport.
User: What documents are required?
Output:
What documents are required to renew a passport?

Conversation:
User: Tell me about the national ID.
User: How much does it cost?
Output:
How much does it cost to issue or renew a national ID?

Conversation:
User: How can I apply for a passport?
Output:
How can I apply for a passport?
"""
            ));

            if (chatSessionDto?.ChatSession_ChatHistory != null)
            {
                foreach (var msg in chatSessionDto.ChatSession_ChatHistory.TakeLast(6))
                {
                    if (msg.Role.Equals("user", StringComparison.OrdinalIgnoreCase))
                    {
                        messages.Add(ChatMessage.CreateUserMessage(msg.Content));
                    }
                    else if (msg.Role.Equals("assistant", StringComparison.OrdinalIgnoreCase))
                    {
                        messages.Add(ChatMessage.CreateAssistantMessage(msg.Content));
                    }
                }
            }

           
            messages.Add(ChatMessage.CreateUserMessage(userQuestion));

            ChatCompletion completion = await _chat.CompleteChatAsync(messages);

            var response = completion.Content[0].Text;
            Console.WriteLine("Rewrite Agent: " + response);

            return response;
        }
    }
}
