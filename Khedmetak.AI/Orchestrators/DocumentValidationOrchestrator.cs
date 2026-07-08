using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khedmetak.AI.Agents.Abstraction;
using Khedmetak.AI.DTOs;
using Khedmetak.AI.DTOs.DocumentValidationDTO;

namespace Khedmetak.AI.Orchestrators;

public class DocumentValidationOrchestrator : IDocumentValidationOrchestrator
{
    private readonly ITemplateComparisonAgent _templateComparisonAgent;
    private readonly IOCRAgent _ocrAgent;
    private readonly IRulesValidationAgent _rulesValidationAgent;

    public DocumentValidationOrchestrator(
        ITemplateComparisonAgent templateComparisonAgent,
        IOCRAgent ocrAgent,
        IRulesValidationAgent rulesValidationAgent)
    {
        _templateComparisonAgent = templateComparisonAgent;
        _ocrAgent = ocrAgent;
        _rulesValidationAgent = rulesValidationAgent;
    }

    public async Task<DocumentValidationResult> ValidateAsync(
        byte[] imageBytes,
        string mediaType,
        byte[]? comparisonImageBytes = null,
        string? comparisonMediaType = null,
        string? expectedDocumentType = null,
        List<string>? rules = null)
    {
        // 1. TemplateComparisonAgent
        var comparisonResult = await _templateComparisonAgent.CompareAsync(
            imageBytes,
            mediaType,
            comparisonImageBytes,
            comparisonMediaType,
            expectedDocumentType);

        // 2. OCRAgent
        var ocrResult = await _ocrAgent.ExtractTextAsync(imageBytes, mediaType);

        // 3. RulesValidationAgent
        var rulesResult = await _rulesValidationAgent.ValidateRulesAsync(ocrResult, rules ?? []);

        // 4. Merge all outputs into DocumentValidationResult
        return MergeResults(
            comparisonResult,
            ocrResult,
            rulesResult,
            comparisonImageBytes is { Length: > 0 },
            expectedDocumentType);
    }

    private static DocumentValidationResult MergeResults(
        TemplateComparisonResult comparison,
        OCRResult ocr,
        RuleValidationResult rules,
        bool comparisonPerformed,
        string? expectedDocumentType)
    {
        var finalResult = new DocumentValidationResult
        {
            DocumentType = comparison.DetectedDocumentType,
            Readable = ocr.Readable,
            ExtractedData = ocr.Fields,
            Confidence = (float)((comparison.Confidence + ocr.Confidence) / 2.0),
            
            MatchesExpectedType = string.IsNullOrWhiteSpace(expectedDocumentType) ? (bool?)null : comparison.MatchesExpectedType,
            ExpectedTypeNote = string.IsNullOrWhiteSpace(expectedDocumentType) ? null : comparison.Summary,

            ComparisonPerformed = comparisonPerformed,
            ComparisonMatch = comparisonPerformed ? comparison.MatchesTemplate : (bool?)null,
            ComparisonSummary = comparisonPerformed ? comparison.Summary : "",

            RuleResults = rules.Results.Select(r => new RuleCheckResult
            {
                Rule = r.Rule,
                Passed = r.Passed,
                Note = r.Note
            }).ToList()
        };

        finalResult.MissingInformation = ocr.MissingFields ?? [];

        var issuesList = new List<string>();
        if (!string.IsNullOrWhiteSpace(expectedDocumentType) && comparison.MatchesExpectedType == false)
        {
            issuesList.Add($"Document type mismatch: Expected '{expectedDocumentType}' but detected '{comparison.DetectedDocumentType}'.");
        }
        if (comparisonPerformed && comparison.MatchesTemplate == false)
        {
            issuesList.Add("The uploaded document does not match the official template layout.");
        }
        if (!ocr.Readable)
        {
            issuesList.Add("The document image is unreadable or blurry.");
        }
        foreach (var ruleResult in finalResult.RuleResults.Where(r => !r.Passed))
        {
            issuesList.Add($"Rule Failed: {ruleResult.Rule}. Note: {ruleResult.Note}");
        }
        finalResult.Issues = issuesList;

        var allRulesPassed = finalResult.RuleResults.All(r => r.Passed);
        var typeMatches = string.IsNullOrWhiteSpace(expectedDocumentType) || comparison.MatchesExpectedType;
        var layoutMatches = !comparisonPerformed || comparison.MatchesTemplate;

        finalResult.AppearsAuthentic = typeMatches && layoutMatches;
        finalResult.PossibleManipulation = comparisonPerformed && !comparison.MatchesTemplate;

        if (!typeMatches)
        {
            finalResult.Status = "UNSUPPORTED_DOCUMENT";
        }
        else if (!ocr.Readable)
        {
            finalResult.Status = "LOW_QUALITY";
        }
        else if (!layoutMatches || !allRulesPassed)
        {
            finalResult.Status = "SUSPICIOUS";
        }
        else
        {
            finalResult.Status = "VALID";
        }

        finalResult.Summary = comparison.Summary;
        if (!ocr.Readable)
        {
            finalResult.Summary += " Document content is not readable.";
        }

        return finalResult;
    }
}
