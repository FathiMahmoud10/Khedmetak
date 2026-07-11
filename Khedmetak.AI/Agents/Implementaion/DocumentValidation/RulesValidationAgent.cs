using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Khedmetak.AI.Agents.Abstraction.DocumentValidation;
using Khedmetak.AI.DTOs;
using Khedmetak.AI.Shared;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace Khedmetak.AI.Agents.Implementaion.DocumentValidation;

public class RulesValidationAgent : IRulesValidationAgent
{
    private readonly IChatClient _chatClient;

    public RulesValidationAgent([FromKeyedServices("DocValidation")] IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<RuleValidationResult> ValidateRulesAsync(OCRResult ocrResult, List<string> rules)
    {
        if (rules == null || rules.Count == 0)
        {
            return new RuleValidationResult();
        }

        // Normalize all OCR values before sending them to the AI
        var normalizedFields = ocrResult.Fields.ToDictionary(
            kv => kv.Key,
            kv => NormalizeDigits(kv.Value)
        );

        var ocrFieldsJson = JsonSerializer.Serialize(
            normalizedFields,
            JsonExtractor.DefaultOptions);

        var rulesList = string.Join(
            Environment.NewLine,
            rules.Select((r, i) => $"{i + 1}. {r}")
        );

        var systemPrompt = """
You are an AI that validates document rules using OCR-extracted field values.

For each rule:
- Evaluate it using only the provided OCR fields.
- Arabic-Indic digits (٠١٢٣٤٥٦٧٨٩) equal Western digits (0-9); values are pre-normalized.
- If a required field is missing, mark the rule as failed and explain why.
- Never invent or assume field values.

Return ONLY this JSON (no markdown, no explanation):
{
  "Results": [
    { "Rule": "...", "Passed": true, "Note": "..." }
  ]
}
""";

        var userPrompt = $"""
OCR Fields:
{ocrFieldsJson}

Rules:
{rulesList}

Evaluate every rule.
""";

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, systemPrompt),
            new(ChatRole.User, userPrompt)
        };

        var options = new ChatOptions
        {
            ResponseFormat = ChatResponseFormat.Json
        };

        var response = await _chatClient.GetResponseAsync(messages, options);

        return JsonExtractor.DeserializeResponse<RuleValidationResult>(response.Text);
    }

    private static string NormalizeDigits(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input ?? string.Empty;

        return input
            .Replace('٠', '0')
            .Replace('١', '1')
            .Replace('٢', '2')
            .Replace('٣', '3')
            .Replace('٤', '4')
            .Replace('٥', '5')
            .Replace('٦', '6')
            .Replace('٧', '7')
            .Replace('٨', '8')
            .Replace('٩', '9')
            // Persian digits
            .Replace('۰', '0')
            .Replace('۱', '1')
            .Replace('۲', '2')
            .Replace('۳', '3')
            .Replace('۴', '4')
            .Replace('۵', '5')
            .Replace('۶', '6')
            .Replace('۷', '7')
            .Replace('۸', '8')
            .Replace('۹', '9');
    }
}