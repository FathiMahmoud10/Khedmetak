using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Khedmetak.AI.Agents.Abstraction.DocumentValidation;
using Khedmetak.AI.DTOs;
using Khedmetak.AI.Shared;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace Khedmetak.AI.Agents.Implementaion.DocumentValidation;

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
You are an AI specialized in validating Egyptian government document images.

Validate IMAGE 1 (the user's uploaded document).

## 1. Document Visibility & Completeness
The uploaded image must contain ONE complete document.

Reject the document if any of the following is detected:
- Any edge or corner is outside the image.
- Any part of the document is cropped, missing, folded, or hidden.
- Hands, fingers, shadows, reflections, or other objects cover any document content.
- Multiple documents appear in the image.
- The document is too small to inspect reliably.

The entire document, including all four corners and all edges, must be clearly visible inside the image.

If the complete document is not visible, immediately set "IsValid" to false.

## 2. Image Quality
The image must be clear enough for reliable inspection.

Reject the document if any of the following is detected:
 Blur or motion blur.
- Low resolution.
- Excessive compression artifacts or noise.
- Poor lighting, overexposure, glare, or heavy shadows.
- Severe perspective distortion.
- Upside-down or excessively rotated image.

If image quality prevents reliable validation, immediately set "IsValid" to false.

## 3. Image Authenticity & Tampering Detection
Determine whether the uploaded image appears to be an authentic, unmodified photo of the original document.

Reject the document if there is evidence of manipulation, including but not limited to:
- Drawn rectangles, boxes, circles, arrows, highlights, or annotations.
- Black boxes, stickers, labels, or overlays.
- Covered, hidden, erased, or masked information.
- Artificial blur or pixelation applied to specific regions.
- Edited, cloned, copied, or digitally altered regions.
- Added or removed text, logos, stamps, signatures, QR codes, or graphics.
- Photoshop, AI editing, or any visible editing artifacts.
- Screenshot of another image instead of a direct photo of the document.
- Cropped screenshot or image taken from another application.
- Any indication that the document has been digitally modified after capture.

If authenticity cannot be trusted, immediately set "IsValid" to false.

## 4. Document Type
Identify the uploaded document and set "DetectedDocumentType".

## 5. Expected Document Check
Expected document:
"{{expectedDocumentName}}"

If the detected document type does not match the expected document, set "IsValid" to false.

""");

        if (hasTemplate)
        {
            sb.Append("""
## 6. Template Comparison (IMAGE 2 = Official Template)

Compare IMAGE 1 with the official template.

Compare ONLY:
- Overall layout
- Headers
- Logos
- Borders
- Colors
- Fonts
- Field positions
- QR/Barcode positions
- Security features
- Overall document structure

Ignore all personalized information, including:
- Personal photo
- Name
- National ID number
- Address
- Dates
- Signature
- Personal numbers
- User-specific data

If the document layout or design does not match the official template, set "IsValid" to false.

""");
        }

        if (hasImageRules)
        {
            var ruleLines = string.Join("\n", imageRules.Select((r, i) => $"{i + 1}. {r}"));

            sb.Append($$"""
## 7. Image Rules

Evaluate IMAGE 1 against these rules:

{{ruleLines}}

For every failed rule:
- Add the exact rule text to "FailedImageRules".
- Add a clear explanation to "ValidationMessages".

""");
        }

        sb.Append("""
Return ONLY this JSON:

{
  "IsValid": true,
  "DetectedDocumentType": "...",
  "FailedImageRules": [],
  "ValidationMessages": []
}

Rules:
- Return ONLY valid JSON.
- Do not include markdown or extra text.
- "IsValid" is true ONLY if every validation succeeds.
- Reject any image that is incomplete, partially visible, manipulated, edited, or of insufficient quality.
- Do not perform template or rule validation if the document is incomplete or image quality/authenticity is insufficient.
- Add one message to "ValidationMessages" for every detected issue.
- "FailedImageRules" contains only the exact image rule texts that failed.
""");

        return sb.ToString();
    }
}
