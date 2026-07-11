using Khedmetak.AI.Agents.Abstraction.Chat;
using Khedmetak.AI.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Khedmetak.AI.Agents.Implementaion.Chat
{
    public class ServiceIntentResult
    {
        public bool IsServiceRequest { get; set; }

        public string Intent { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public double Confidence { get; set; }

        public string Reasoning { get; set; } = string.Empty;
    }
    public class ServiceIntentAgent : IServiceIntentAgent
    {
        private readonly ChatClient _chat;

        public ServiceIntentAgent([FromKeyedServices("github")] OpenAIClient githubClient, IOptions<AISettings> settings)
        {
            _chat = githubClient.GetChatClient(settings.Value.Model);
        }

        public async Task<ServiceIntentResult> DetectIntentAsync( string userMessage)
        {
            var prompt = $$"""
You are a governmental service intent detection agent.

Your task:
1. Understand the user's real goal.
2. Ignore irrelevant details.
3. Determine whether the user is asking about a governmental service.
4. Convert the request into a short canonical service query.

IMPORTANT:
- Return ONLY valid JSON.
- Do not use markdown.
- Do not use ```json.
- Do not add explanations.
- Do not add text before or after the JSON.

Example 1:

{
  "isServiceRequest": true,
  "intent": "استخراج جواز سفر",
  "category": "Passport",
  "confidence": 0.98,
  "reasoning": "User wants to obtain a passport."
}

Example 2:

{
  "isServiceRequest": false,
  "intent": "",
  "category": "",
  "confidence": 1.0,
  "reasoning": "Greeting only."
}

User:
{{userMessage}}
""";

            //Console.WriteLine("Before Request");
            var response = await _chat.CompleteChatAsync(prompt);
            //Console.WriteLine("After Request");

            var content = response.Value.Content[0].Text.Trim();

            // Remove markdown if model returns it anyway
            content = content
                .Replace("```json", "")
                .Replace("```", "")
                .Trim();

            Console.WriteLine("Intent Agent Response:");
            Console.WriteLine(content);

            try
            {
                return JsonSerializer.Deserialize<ServiceIntentResult>(
                    content,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    })!;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Failed to parse intent JSON.\nResponse:\n{content}",
                    ex);
            }
        }
    }
}
