using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Khedmetak.AI.Agents.Abstraction;
using Khedmetak.AI.DTOs;
using Khedmetak.AI.Shared;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace Khedmetak.AI.Agents.Implementation;

/// <summary>
/// Validates document images by combining template layout comparison,
/// image quality checks, and image-based rule evaluation in a single AI call.
/// </summary>
public class TemplatesAgent : ITemplatesAgent
{
    private readonly IChatClient _chatClient;

    public TemplatesAgent([FromKeyedServices("DocValidation")] IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<ImageValidationResult> ValidateAsync(
        byte[] userDocumentBytes,
        string mediaType,
        byte[]? templateImageBytes,
        string? templateMediaType,
        string expectedDocumentName,
        List<string> imageRules)
    {
        var hasTemplate = templateImageBytes is { Length: > 0 };
        var hasImageRules = imageRules.Count > 0;

        var systemPrompt = BuildSystemPrompt(hasTemplate, hasImageRules, imageRules, expectedDocumentName);

        var userContent = new List<AIContent>
        {
            new TextContent($"Analyze this document (IMAGE 1). It should be: \"{expectedDocumentName}\"."),
            new DataContent(userDocumentBytes, mediaType)
        };

        if (hasTemplate)
        {
            userContent.Add(new TextContent("IMAGE 2 is the official template. Compare the uploaded document layout against it."));
            userContent.Add(new DataContent(templateImageBytes!, templateMediaType ?? mediaType));
        }

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, systemPrompt),
            new(ChatRole.User, userContent.ToArray())
        };

        var options = new ChatOptions { ResponseFormat = ChatResponseFormat.Json };
        var response = await _chatClient.GetResponseAsync(messages, options);
        return JsonExtractor.DeserializeResponse<ImageValidationResult>(response.Text);
    }

    private static string BuildSystemPrompt(
        bool hasTemplate,
        bool hasImageRules,
        List<string> imageRules,
        string expectedDocumentName)
    {
        var sb = new StringBuilder();

        sb.Append($$"""
You are an AI that validates Egyptian government document images.

Tasks for IMAGE 1 (the user's uploaded document):

## 1. Image Quality
Detect any of these problems:
- Blurry, out of focus, motion blur
- Cropped, partial, or missing edges/corners
- Rotated, tilted, or upside-down
- Excessive glare or reflections
- Heavy shadows hiding important areas
- Low resolution or heavy noise/artifacts
- Too dark or overexposed
- Document occupies too small a portion of the image
- Fingers or objects covering important content
- Multiple documents in one image
If any problem prevents reliable reading, set "IsValid" to false and add a message.

## 2. Document Type
Detect the document type. Set "DetectedDocumentType".

## 3. Expected Type Check
The expected document is: "{{expectedDocumentName}}".
If the detected type does not match, set "IsValid" to false and add a message.

""");

        if (hasTemplate)
        {
            sb.Append("""
## 4. Template Comparison (IMAGE 2 = official template)
Compare IMAGE 1 layout against the official template.
Compare ONLY: layout, field arrangement, headers, logos, colors, borders, fonts, QR/barcode positions, security features.
IGNORE completely: photo, name, ID number, address, dates, signature — any personalized data.
If the layout does not match the template, set "IsValid" to false and add a message.

""");
        }

        if (hasImageRules)
        {
            var ruleLines = string.Join("\n", imageRules.Select((r, i) => $"{i + 1}. {r}"));
            sb.Append($"""
## 5. Image Rules
Evaluate each rule against IMAGE 1:
{ruleLines}
For each failed rule: add its text to "FailedImageRules" and a human-readable explanation to "ValidationMessages".

""");
        }

        sb.Append("""
Return ONLY this JSON (no markdown, no extra text):
{
  "IsValid": true,
  "DetectedDocumentType": "...",
  "FailedImageRules": [],
  "ValidationMessages": []
}
- "IsValid" is true only if ALL checks above pass.
- "ValidationMessages" lists every problem found (one entry per issue).
- "FailedImageRules" lists only the image rule texts that failed.
""");

        return sb.ToString();
    }
}
