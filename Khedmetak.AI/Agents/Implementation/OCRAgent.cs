using System.Collections.Generic;
using System.Threading.Tasks;
using Khedmetak.AI.Agents.Abstraction;
using Khedmetak.AI.DTOs;
using Khedmetak.AI.Shared;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace Khedmetak.AI.Agents.Implementation;

public class OCRAgent : IOCRAgent
{
    private readonly IChatClient _chatClient;

    public OCRAgent([FromKeyedServices("DocValidation")] IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<OCRResult> ExtractTextAsync(byte[] imageBytes, string mediaType)
    {
        var systemPrompt = """
You are an AI specialized in performing optical character recognition (OCR) on Egyptian government documents.
Your task is to extract every visible field and value from the uploaded document.

CRITICAL RULES FOR NUMBERS:
- Egyptian National ID numbers are EXACTLY 14 digits. Serial/document numbers may also be long (9-14+ digits).
- Read long digit sequences ONE DIGIT AT A TIME, left to right. Do not estimate, round, or infer digits from context.
- Do NOT stop early or truncate a digit sequence just because it "looks long enough" — continue until the printed sequence visually ends.
- Documents may contain Arabic-Indic numerals or Western numerals.
- Convert every Arabic-Indic digit to its Western equivalent using this exact mapping:
  - ٠ → 0
  - ١ → 1
  - ٢ → 2
  - ٣ → 3
  - ٤ → 4
  - ٥ → 5
  - ٦ → 6
  - ٧ → 7
  - ٨ → 8
  - ٩ → 9
- Always output numeric values using Western digits (0-9) only.
- Before finalizing each numeric field, recount the digits you extracted against what is visibly printed, and correct any mismatch.
- If a digit is unclear or partially occluded, still provide your best single-digit guess — never omit digits from the middle of a sequence.

Do NOT compare documents, do NOT compare templates, do NOT detect document type, do NOT validate rules, and do NOT attempt to detect manipulation. Focus solely on extracting readable text fields and their values.

Return ONLY JSON matching this format:
{
  "Readable": true,
  "Fields": {
    "Field Name 1": "Value 1",
    "Field Name 2": "Value 2"
  },
  "MissingFields": ["Field Name A", "Field Name B"],
  "Confidence": 0.0
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
            new TextContent("Extract fields and text from this document image."),
            new DataContent(imageBytes, mediaType)
        };

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
        return JsonExtractor.DeserializeResponse<OCRResult>(response.Text);
    }
}
