using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Khedmetak.AI.Agents.Abstraction;
using Khedmetak.AI.DTOs;
using Khedmetak.AI.Shared;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Khedmetak.AI.Agents.Implementation;

public class OCRAgent : IOCRAgent
{
    private readonly IChatClient _chatClient;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public OCRAgent(
        [FromKeyedServices("DocValidation")] IChatClient chatClient,
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _chatClient = chatClient;
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<OCRResult> ExtractTextAsync(byte[] imageBytes, string mediaType)
    {
        try
        {
            // 1. Call OCR.Space API to extract raw full text
            var rawText = await CallOcrSpaceAsync(imageBytes, mediaType);

            if (string.IsNullOrWhiteSpace(rawText))
            {
                return new OCRResult
                {
                    Readable = false,
                    FullText = string.Empty,
                    Fields = [],
                    MissingFields = [],
                    Confidence = 0.0
                };
            }

            // 2. Normalize raw text (Arabic-Indic to Western digits, whitespace trimming, preserve Arabic)
            var normalizedText = NormalizeOcrText(rawText);

            // 3. Prompt the AI with ONLY the normalized text (no image) to structure it into key/value fields
            var systemPrompt = """
You are an OCR structuring AI for Egyptian government documents.

Extract all key/value fields from the provided text.
Rules:
- Extract values exactly as they appear — never invent or guess.
- If the text is too garbled to extract any fields, set "Readable" to false.
- List any fields that appear to be missing or incomplete in "MissingFields".

Return ONLY this JSON (no markdown, no explanation):
{
  "Readable": true,
  "Fields": { "Field Name": "Value" },
  "MissingFields": ["..."],
  "Confidence": 1.0
}
""";

            var userPrompt = $"""
Extracted OCR Text:
{normalizedText}

Please extract all fields and values from this text.
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
            var ocrResult = JsonExtractor.DeserializeResponse<OCRResult>(response.Text);

            // Set the raw/normalized full text on the final result
            ocrResult.FullText = normalizedText;

            return ocrResult;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during OCR.Space validation/AI structuring: {ex}");
            return new OCRResult
            {
                Readable = false,
                FullText = string.Empty,
                Fields = [],
                MissingFields = [],
                Confidence = 0.0
            };
        }
    }

    private async Task<string> CallOcrSpaceAsync(byte[] imageBytes, string mediaType)
    {
        var apiKey = _configuration["AI:OcrSpaceApiKey"] ?? "helloworld";
        var language = _configuration["AI:OcrSpaceLanguage"] ?? "ara";
        var ocrEngine = _configuration["AI:OcrSpaceEngine"] ?? "3";

        using var requestContent = new MultipartFormDataContent();
        
        var fileContent = new ByteArrayContent(imageBytes);
        fileContent.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(mediaType);
        
        string fileName = mediaType switch
        {
            "image/png" => "image.png",
            "image/gif" => "image.gif",
            "image/webp" => "image.webp",
            "application/pdf" => "image.pdf",
            _ => "image.jpg"
        };

        requestContent.Add(fileContent, "file", fileName);
        requestContent.Add(new StringContent(apiKey), "apikey");
        requestContent.Add(new StringContent(language), "language");
        requestContent.Add(new StringContent(ocrEngine), "ocrengine");
        requestContent.Add(new StringContent("true"), "detectOrientation");
        requestContent.Add(new StringContent("true"), "scale");

        var response = await _httpClient.PostAsync("https://api.ocr.space/parse/image", requestContent);
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"OCR.Space API call failed with status code: {response.StatusCode}");
            return string.Empty;
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        var ocrResponse = JsonSerializer.Deserialize<OcrSpaceResponse>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (ocrResponse == null || ocrResponse.IsErroredOnProcessing || ocrResponse.ParsedResults == null)
        {
            Console.WriteLine($"OCR.Space error: ExitCode={ocrResponse?.OCRExitCode}, Message={ocrResponse?.ErrorMessage}");
            return string.Empty;
        }

        var sb = new StringBuilder();
        foreach (var result in ocrResponse.ParsedResults)
        {
            if (!string.IsNullOrEmpty(result.ParsedText))
            {
                sb.AppendLine(result.ParsedText);
            }
        }

        return sb.ToString();
    }

    private static string NormalizeOcrText(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Convert Arabic-Indic digits to Western digits
        var sb = new StringBuilder(input.Length);
        foreach (var c in input)
        {
            switch (c)
            {
                case '٠': sb.Append('0'); break;
                case '١': sb.Append('1'); break;
                case '٢': sb.Append('2'); break;
                case '٣': sb.Append('3'); break;
                case '٤': sb.Append('4'); break;
                case '٥': sb.Append('5'); break;
                case '٦': sb.Append('6'); break;
                case '٧': sb.Append('7'); break;
                case '٨': sb.Append('8'); break;
                case '٩': sb.Append('9'); break;
                // Persian digits
                case '۰': sb.Append('0'); break;
                case '۱': sb.Append('1'); break;
                case '۲': sb.Append('2'); break;
                case '۳': sb.Append('3'); break;
                case '۴': sb.Append('4'); break;
                case '۵': sb.Append('5'); break;
                case '۶': sb.Append('6'); break;
                case '۷': sb.Append('7'); break;
                case '۸': sb.Append('8'); break;
                case '۹': sb.Append('9'); break;
                default: sb.Append(c); break;
            }
        }

        var normalizedDigits = sb.ToString();

        // Trim unnecessary whitespace line-by-line
        var lines = normalizedDigits.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        var cleanedLines = new List<string>();

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            if (string.IsNullOrEmpty(trimmedLine))
                continue;

            // Replace multiple spaces/tabs with a single space
            var words = trimmedLine.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            cleanedLines.Add(string.Join(" ", words));
        }

        return string.Join(Environment.NewLine, cleanedLines);
    }
}

public class OcrSpaceResponse
{
    public List<OcrSpaceParsedResult>? ParsedResults { get; set; }
    public int OCRExitCode { get; set; }
    public bool IsErroredOnProcessing { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ErrorDetails { get; set; }
}

public class OcrSpaceParsedResult
{
    public string? ParsedText { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ErrorDetails { get; set; }
}
