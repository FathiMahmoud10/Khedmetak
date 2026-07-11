using Khedmetak.AI.Agents.Abstraction.Chat;
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

namespace Khedmetak.AI.Agents.Implementaion.Chat
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

              Your task is to rewrite the user's latest message into a standalone question ONLY when necessary.

              Rules:

              1. If the user's latest message is already a complete, standalone question that can be understood without previous messages, return it EXACTLY as written.
              2. Rewrite the latest message ONLY when it depends on previous conversation (for example: pronouns like "it", "that", "those", "them", or follow-up questions such as "What about the documents?", "How much does it cost?", "Can I renew it online?").
              3. Use previous conversation ONLY to resolve missing references. Never use it to change the language, wording, or intent unless required for clarification.
              4. Preserve the user's original intent exactly. Do not answer the question.
              5. Do not add, remove, or assume information that is not clearly implied by the conversation.
              6. Return ONLY the rewritten question. Do not explain your reasoning. Do not use quotation marks or markdown.

              Language Rules (Highest Priority):

              - Determine the language ONLY from the user's latest message.
              - Ignore the language used in previous conversation.
              - Never translate the user's message.
              - Preserve the language exactly.
              - If the latest message is English, the output MUST be English.
              - If the latest message is Arabic, the output MUST be Arabic.
              - Preserve the user's dialect when the latest message is in a dialect.
              - Preserve the user's level of formality.

              Examples:

              Conversation:
              User: أريد تجديد جواز السفر.
              User: What documents are required?
              Output:
              What documents are required to renew a passport?

              Conversation:
              User: I want to renew my passport.
              User: ما هي المستندات المطلوبة؟
              Output:
              ما هي المستندات المطلوبة لتجديد جواز السفر؟

              Conversation:
              User: I want to renew my passport.
              User: What documents are required?
              Output:
              What documents are required to renew a passport?

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
