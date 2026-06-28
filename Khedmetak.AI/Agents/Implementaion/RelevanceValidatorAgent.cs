using Khedmetak.AI.Agents.Abstraction;
using Khedmetak.AI.DTOs.RagDTOs;
using Khedmetak.AI.RAG;
using Microsoft.Extensions.AI;
using System.Text.Json;

namespace Khedmetak.AI.Agents.Implementation
{
    public class RelevanceValidatorAgent : IRelevanceValidatorAgent
    {
        private readonly IChatClient _chatClient;

        public RelevanceValidatorAgent(IChatClient chatClient)
        {
            _chatClient = chatClient;
        }

        public async Task<bool> IsRelevantAsync(string userQuestion, RagServiceInfo serviceInfo)
        {
            var prompt = $"""
                أنت محكّم دقيق. مهمتك فقط: هل سؤال المستخدم يتعلق بالخدمة الحكومية المسترجعة؟

                سؤال المستخدم: {userQuestion}

                الخدمة المسترجعة:
                - اسم الخدمة: {serviceInfo.ServiceName}

                قواعد التقييم:
                - أجب بـ true إذا كان السؤال يستفسر عن هذه الخدمة تحديداً أو خدمة مشابهة جداً لها.
                - أجب بـ false إذا كانت الخدمة المسترجعة لا تتوافق مع نية المستخدم الحقيقية.
                - لا تعتمد فقط على تشابه الكلمات — حكّم على المعنى.

                أجب فقط بـ JSON بهذا الشكل بدون أي نص إضافي:
                {"isRelevant": true}  أو  {"isRelevant": false}
                """;

            var messages = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System,
                    "أنت محكّم دقيق يجيب فقط بـ JSON صحيح بدون أي نص إضافي."),
                new ChatMessage(ChatRole.User, prompt)
            };

            var response = await _chatClient.GetResponseAsync(messages);
            var raw = response.Text;

            try
            {
                // Strip markdown fences if model wraps output in ```json ... ```
                var clean = raw
                    .Replace("```json", "")
                    .Replace("```", "")
                    .Trim();

                var result = JsonSerializer.Deserialize<JsonElement>(clean);
                return result.GetProperty("isRelevant").GetBoolean();
            }
            catch
            {
                return true; // fail-open: don't block valid queries on parse errors
            }
        }
    }
}