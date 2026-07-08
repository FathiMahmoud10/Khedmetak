using System.Collections.Generic;
using System.Threading.Tasks;
using Khedmetak.AI.Agents.Abstraction;
using Khedmetak.AI.DTOs;
using Khedmetak.AI.Shared;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace Khedmetak.AI.Agents.Implementation;

public class TemplateComparisonAgent : ITemplateComparisonAgent
{
    private readonly IChatClient _chatClient;

    public TemplateComparisonAgent([FromKeyedServices("DocValidation")] IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<TemplateComparisonResult> CompareAsync(
        byte[] imageBytes,
        string mediaType,
        byte[]? comparisonImageBytes = null,
        string? comparisonMediaType = null,
        string? expectedDocumentType = null)
    {
        var hasComparison = comparisonImageBytes is { Length: > 0 };

        var systemPrompt = $$"""
You are an AI specialized in analyzing and comparing Egyptian government documents.

Supported documents include:
- National ID
- Passport
- Birth Certificate
- Marriage Certificate
- Divorce Certificate
- Death Certificate
- Driving License
- Vehicle License
- Military Certificate
- Graduation Certificate
- Government permits
- Tax documents
- Insurance documents
- Any Egyptian government document

Your responsibilities:
1. Detect the uploaded document type (IMAGE 1).
2. Verify if the detected type matches the expected document type: "{expectedDocumentType}" (if provided).
3. If a reference template image (IMAGE 2) is provided:
   - Compare the layout and structure of the uploaded document (IMAGE 1) against the official template (IMAGE 2).
   - Compare ONLY: layout, field arrangement, headers, logos, colors, fonts, borders, labels, QR position, barcode position, security feature locations, document sections.
   - Ignore completely: photo, face, signature, name, ID number, address, dates, any personalized information. Personalized information must NEVER affect the comparison.

Return ONLY JSON matching this format:
{
  "DetectedDocumentType": "...",
  "MatchesExpectedType": true/false,
  "MatchesTemplate": true/false,
  "Confidence": 0.0,
  "Summary": "..."
}

IMPORTANT RULES:
- Return ONLY JSON.
- Do NOT write any explanation.
- Do NOT use Markdown.
- Do NOT wrap the response in ```json.
- Do NOT add text before or after the JSON.
""";

        var userContent = new List<AIContent>
        {
            new TextContent("Analyze this document (IMAGE 1 - the primary document)."),
            new DataContent(imageBytes, mediaType)
        };

        if (hasComparison)
        {
            userContent.Add(new TextContent("This is IMAGE 2 - compare it against IMAGE 1 as instructed."));
            userContent.Add(new DataContent(comparisonImageBytes!, comparisonMediaType ?? mediaType));
        }
        else
        {
            userContent.Add(new TextContent("No comparison template was provided. Set MatchesTemplate to false."));
        }

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, systemPrompt),
            new(ChatRole.User, userContent.ToArray())
        };

        var options = new ChatOptions
        {
            ResponseFormat = ChatResponseFormat.Json
        };

        var response = await _chatClient.GetResponseAsync(messages, options);
        return JsonExtractor.DeserializeResponse<TemplateComparisonResult>(response.Text);
    }
}
