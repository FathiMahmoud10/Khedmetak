using System;
using System.Text.Json;

namespace Khedmetak.AI.Shared;

public static class JsonExtractor
{
    public static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static string ExtractJson(string? response)
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

    public static T DeserializeResponse<T>(string? response)
    {
        var json = ExtractJson(response);
        var result = JsonSerializer.Deserialize<T>(json, DefaultOptions);
        if (result == null)
        {
            throw new InvalidOperationException("Deserialization returned null.");
        }
        return result;
    }
}
