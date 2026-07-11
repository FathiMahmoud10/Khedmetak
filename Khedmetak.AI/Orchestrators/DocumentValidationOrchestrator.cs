using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khedmetak.AI.Agents.Abstraction.DocumentValidation;
using Khedmetak.AI.DTOs.DocumentValidationDTO;

namespace Khedmetak.AI.Orchestrators;

public class DocumentValidationOrchestrator : IDocumentValidationOrchestrator
{
    private readonly ITemplatesAgent _templatesAgent;
    private readonly IOCRAgent _ocrAgent;
    private readonly IRulesValidationAgent _rulesValidationAgent;

    // Keywords that identify a rule as image-based rather than text/OCR-based.
    // Rules containing any of these substrings (case-insensitive) are routed to TemplatesAgent.
    private static readonly string[] ImageRuleKeywords =
    [
        "background", "white", "visible", "cropp", "rotati", "tilt",
        "blur", "glare", "lighting", "light", "orient", "resolution",
        "distort", "shadow", "partial", "entire", "quality", "reflect",
        "focus", "dark", "bright", "overexpos", "noise", "image", "photo"
    ];

    public DocumentValidationOrchestrator(
        ITemplatesAgent templatesAgent,
        IOCRAgent ocrAgent,
        IRulesValidationAgent rulesValidationAgent)
    {
        _templatesAgent = templatesAgent;
        _ocrAgent = ocrAgent;
        _rulesValidationAgent = rulesValidationAgent;
    }

    public async Task<DocumentValidationResult> ValidateAsync(
        byte[] userDocumentBytes,
        string mediaType,
        string expectedDocumentName,
        byte[]? templateImageBytes,
        string? templateMediaType,
        List<string> rules)
    {
        // ── 1. Classify rules ─────────────────────────────────────────────────
        var imageRules = rules.Where(IsImageRule).ToList();
        var textRules  = rules.Where(r => !IsImageRule(r)).ToList();

        // ── 2. TemplatesAgent — always runs ───────────────────────────────────
        //    Validates image quality, template layout match, and image rules.
        var imageResult = await _templatesAgent.ValidateAsync(
            userDocumentBytes,
            mediaType,
            templateImageBytes,
            templateMediaType,
            expectedDocumentName,
            imageRules);

        if (!imageResult.IsValid)
        {
            // Early exit — do not proceed to OCR when the image itself is invalid.
            return new DocumentValidationResult
            {
                IsValid = false,
                DocumentType = imageResult.DetectedDocumentType,
                ValidationErrors = imageResult.ValidationMessages.Count > 0
                    ? imageResult.ValidationMessages
                    : ["Image validation failed. Please upload a clear, unobstructed photo of the document."]
            };
        }

        // ── 3. Skip OCR when no text rules exist ──────────────────────────────
        if (textRules.Count == 0)
        {
            return new DocumentValidationResult
            {
                IsValid = true,
                DocumentType = imageResult.DetectedDocumentType
            };
        }

        // ── 4. OCRAgent — extract text and structure into fields ───────────────
        var ocrResult = await _ocrAgent.ExtractTextAsync(userDocumentBytes, mediaType);

        if (!ocrResult.Readable)
        {
            return new DocumentValidationResult
            {
                IsValid = false,
                DocumentType = imageResult.DetectedDocumentType,
                ValidationErrors = ["The document text could not be read. Please upload a clearer image."]
            };
        }

        // ── 5. RulesValidationAgent — evaluate text-based rules ────────────────
        var rulesResult = await _rulesValidationAgent.ValidateRulesAsync(ocrResult, textRules);

        var errors = rulesResult.Results
            .Where(r => !r.Passed)
            .Select(r => string.IsNullOrWhiteSpace(r.Note) ? $"Rule failed: {r.Rule}" : r.Note)
            .ToList();

        // ── 6. Compose final result ────────────────────────────────────────────
        return new DocumentValidationResult
        {
            IsValid        = errors.Count == 0,
            DocumentType   = imageResult.DetectedDocumentType,
            ValidationErrors = errors,
            ExtractedFields = ocrResult.Fields.Count > 0 ? ocrResult.Fields : null
        };
    }

    // A rule is image-based if it contains any of the known visual keywords.
    private static bool IsImageRule(string rule) =>
        ImageRuleKeywords.Any(kw => rule.Contains(kw, StringComparison.OrdinalIgnoreCase));
}
