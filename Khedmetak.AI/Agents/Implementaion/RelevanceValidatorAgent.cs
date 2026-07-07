using Khedmetak.AI.Agents.Abstraction;
//using Khedmetak.AI.DTOs.RagDTOs;
using Shard.DTOS;
using Khedmetak.AI.RAG;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace Khedmetak.AI.Agents.Implementation
{
    public class RelevanceValidatorAgent : IRelevanceValidatorAgent
    {
        private readonly IChatClient _chatClient;

        public RelevanceValidatorAgent([FromKeyedServices("Chat")] IChatClient chatClient)
        {
            _chatClient = chatClient;
        }

        public async Task<bool> IsRelevantAsync(string userQuestion, RagServiceInfo serviceInfo)
        {
            var systemPrompt = """
You are a strict relevance validator for Egyptian government services.

Your task is to determine whether the retrieved service is the EXACT government service requested by the user.

Evaluation rules:
- Compare the user's intent with the retrieved service based on meaning, purpose, and government operation.
- Ignore wording differences, synonyms, abbreviations, and common alternative expressions.
- Return true ONLY if both refer to the same government service and same operation.
- Return false if they differ in operation, even if they belong to the same service family (e.g. New, Renewal, Replacement, Lost, Damaged, Update, Correction, Cancellation).
- Return false if the retrieved service is merely related, similar, a prerequisite, a follow-up, or an alternative service.
- If there is any uncertainty, return false.

Output rules:
- Respond with exactly one valid JSON object.
- The JSON must have a single property named "isRelevant".
- Do not include explanations, markdown, code fences, or any additional text.

Valid responses:
{"isRelevant": true}
{"isRelevant": false}
""";

            var userPrompt = $"""
User Question:
{userQuestion}

Retrieved Service:
{serviceInfo.ServiceName}
""";

            var messages = new List<ChatMessage>
{
    new(ChatRole.System, systemPrompt),
    new(ChatRole.User, userPrompt)
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
                Console.WriteLine(result);
                return result.GetProperty("isRelevant").GetBoolean();
            }
            catch
            {
                return true; // fail-open: don't block valid queries on parse errors
            }
        }
    }
}