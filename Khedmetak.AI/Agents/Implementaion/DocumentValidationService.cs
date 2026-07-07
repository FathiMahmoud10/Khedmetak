using System.Text;
using System.Text.Json;
using Khedmetak.AI.Agents.Abstraction;
using Khedmetak.AI.DTOs.DocumentValidationDTO;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace Khedmetak.AI.Agents.Implementaion;

public class DocumentValidationService : IDocumentValidationService
{
    private readonly IChatClient _chatClient;

    public DocumentValidationService(
        [FromKeyedServices("DocValidation")] IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<DocumentValidationResult> ValidateAsync(
        byte[] imageBytes,
        string mediaType,
        byte[]? comparisonImageBytes = null,
        string? comparisonMediaType = null,
        string? expectedDocumentType = null,
        List<string>? rules = null)
    {
        var hasComparison = comparisonImageBytes is { Length: > 0 };
        var hasExpectedType = !string.IsNullOrWhiteSpace(expectedDocumentType);
        var hasRules = rules is { Count: > 0 };

        var systemPrompt = BuildSystemPrompt(hasComparison, hasExpectedType, hasRules, expectedDocumentType, rules);

        var userContent = BuildUserContent(imageBytes, mediaType, comparisonImageBytes, comparisonMediaType, hasComparison);

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, systemPrompt),
            new(ChatRole.User, userContent)
        };

        var options = new ChatOptions
        {
            ResponseFormat = ChatResponseFormat.Json
        };

        var response = await _chatClient.GetResponseAsync(
            messages,
            options);

        Console.WriteLine("========== MODEL RESPONSE ==========");
        Console.WriteLine(response.Text);

        var json = ExtractJson(response.Text);

        Console.WriteLine("========== EXTRACTED JSON ==========");
        Console.WriteLine(json);

        try
        {
            var result = JsonSerializer.Deserialize<DocumentValidationResult>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (result == null)
                throw new Exception("Model returned null.");

            return result;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to parse model response.\n\nRaw Response:\n{response.Text}\n\nExtracted JSON:\n{json}",
                ex);
        }
    }

    private static string BuildSystemPrompt(
        bool hasComparison,
        bool hasExpectedType,
        bool hasRules,
        string? expectedDocumentType,
        List<string>? rules)
    {
        var sb = new StringBuilder();

        sb.AppendLine("""
You are an AI specialized in validating Egyptian government documents.

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

IMPORTANT RULES

- Return ONLY JSON.
- Do NOT write any explanation.
- Do NOT use Markdown.
- Do NOT wrap the response in ```json.
- Do NOT add text before or after the JSON.
- Never say "Here is the JSON".

Never claim a document is definitely genuine or fake.
Only judge using visible evidence.

Status MUST be exactly one of:

VALID
SUSPICIOUS
LOW_QUALITY
UNSUPPORTED_DOCUMENT
""");

        if (hasExpectedType)
        {
            sb.AppendLine($"""

EXPECTED DOCUMENT TYPE CHECK

The caller expects this image to be a: "{expectedDocumentType}".
Compare the actual document you see against this expected type.
Set "matchesExpectedType" to true only if the uploaded document genuinely is that type.
Set "expectedTypeNote" to a short explanation of why it matches or doesn't.
""");
        }

        if (hasComparison)
        {
            sb.AppendLine("""

REFERENCE TEMPLATE DOCUMENT

You will receive TWO images.

IMAGE 1:
The user's uploaded document.

IMAGE 2:
An official reference/template image of the expected document type.

The template exists ONLY to verify that the uploaded document follows the
expected official layout and structure.

Compare ONLY:

- overall layout
- field arrangement
- document dimensions/aspect ratio
- headers
- logos
- emblems
- colors
- labels
- fonts
- security feature locations
- QR/barcode locations
- stamps or seals (if part of the template)
- borders
- sections

IGNORE completely:

- personal photo
- person's face
- signature
- ID number
- name
- address
- birth date
- issue date
- expiry date
- any personalized information

Differences caused by personalization MUST NOT be considered mismatches.

Determine whether the uploaded document visually follows the official template.

Set:

comparisonPerformed = true

comparisonMatch = true only if the uploaded document matches the official
document layout and design.

comparisonSummary should briefly explain which structural elements matched or
did not match.

Do NOT compare faces between the two images.
Do NOT compare personal information.
Do NOT compare signatures.
""");
        }
        else
        {
            sb.AppendLine("""

There is no second image in this request. Set "comparisonPerformed" to false,
"comparisonMatch" to null, and "comparisonSummary" to an empty string.
""");
        }

        if (hasRules)
        {
            var rulesList = string.Join("\n", rules!.Select((r, i) => $"{i + 1}. {r}"));
            sb.AppendLine($"""

CUSTOM RULES

Evaluate the document against EACH of the following rules independently.
For every rule, return one object in "ruleResults" with:
- "rule": the exact rule text as given below
- "passed": true or false, based only on visible evidence
- "note": a short explanation of why it passed or failed

Rules to evaluate:
{rulesList}
""");
        }

        sb.AppendLine("""

Return exactly this schema (use null/false/empty values for any section that does not apply):

{
  "documentType": "",
  "status": "",
  "confidence": 0.0,
  "readable": true,
  "appearsAuthentic": true,
  "possibleManipulation": false,
  "missingInformation": [],
  "issues": [],
  "securityFeaturesVisible": [],
  "extractedData": {},
  "summary": "",
  "matchesExpectedType": null,
  "expectedTypeNote": "",
  "comparisonPerformed": false,
  "comparisonMatch": null,
  "comparisonSummary": "",
  "ruleResults": []
}
""");

        return sb.ToString();
    }

    private static AIContent[] BuildUserContent(
        byte[] imageBytes,
        string mediaType,
        byte[]? comparisonImageBytes,
        string? comparisonMediaType,
        bool hasComparison)
    {
        var content = new List<AIContent>
        {
            new TextContent("""
Analyze this document (IMAGE 1 - the primary document).

Return ONLY JSON.

No explanation.
No markdown.
No extra text.
"""),
            new DataContent(imageBytes, mediaType)
        };

        if (hasComparison)
        {
            content.Add(new TextContent(
                "This is IMAGE 2 - compare it against IMAGE 1 as instructed."));
            content.Add(new DataContent(comparisonImageBytes!, comparisonMediaType ?? mediaType));
        }

        return content.ToArray();
    }

    private static string ExtractJson(string? response)
    {
        if (string.IsNullOrWhiteSpace(response))
            throw new InvalidOperationException("Empty model response.");

        var text = response.Trim();

        text = text
            .Replace("```json", "", StringComparison.OrdinalIgnoreCase)
            .Replace("```", "")
            .Trim();

        var start = text.IndexOf('{');

        if (start < 0)
            throw new InvalidOperationException(
                $"No JSON object found.\n\n{text}");

        int depth = 0;

        for (int i = start; i < text.Length; i++)
        {
            if (text[i] == '{')
                depth++;

            if (text[i] == '}')
            {
                depth--;

                if (depth == 0)
                    return text.Substring(start, i - start + 1);
            }
        }

        throw new InvalidOperationException(
            $"Incomplete JSON object.\n\n{text}");
    }
}