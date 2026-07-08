using Khedmetak.AI.DTOs.DocumentValidationDTO;
using Khedmetak.AI.Orchestrators;
using Khedmetak.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Khedmetak.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentValidationController : ControllerBase
{
    private readonly IDocumentValidationOrchestrator _orchestrator;
    private readonly IRequiredDocumentRepository _requiredDocumentRepository;

    public DocumentValidationController(
        IDocumentValidationOrchestrator orchestrator,
        IRequiredDocumentRepository requiredDocumentRepository)
    {
        _orchestrator = orchestrator;
        _requiredDocumentRepository = requiredDocumentRepository;
    }

    /// <summary>
    /// Validates an uploaded document against the configured rules and template
    /// for the specified required document. The client only supplies the file
    /// and the required document ID — all metadata is loaded from the database.
    /// </summary>
    [HttpPost("validate")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Validate([FromForm] DocumentValidationRequest request)
    {
        if (request.UserDocument == null || request.UserDocument.Length == 0)
            return BadRequest("No file uploaded.");

        // ── Load RequiredDocument and its related StandardDocument ─────────────
        var requiredDoc = await _requiredDocumentRepository.GetByIdAsync(
            request.RequiredDocumentId,
            d => d.StandardDocument!);

        if (requiredDoc == null)
            return NotFound($"Required document with ID {request.RequiredDocumentId} was not found.");

        var standardDoc = requiredDoc.StandardDocument;

        // Prefer the StandardDocument name; fall back to the RequiredDocument name.
        var documentName = !string.IsNullOrWhiteSpace(standardDoc?.DocumentName)
            ? standardDoc!.DocumentName
            : requiredDoc.DocumentName;

        // ── Build rules list from GeneralRule string ──────────────────────────
        var rules = ParseRules(standardDoc?.GeneralRule);

        // ── Load template image bytes from disk (if stored) ───────────────────
        byte[]? templateBytes = null;
        string? templateMediaType = null;

        if (standardDoc != null && !string.IsNullOrWhiteSpace(standardDoc.ImagePath))
        {
            var fullPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                standardDoc.ImagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            if (System.IO.File.Exists(fullPath))
            {
                templateBytes    = await System.IO.File.ReadAllBytesAsync(fullPath);
                templateMediaType = ResolveMediaType(fullPath);
            }
        }

        // ── Read uploaded document bytes ──────────────────────────────────────
        await using var stream = new MemoryStream();
        await request.UserDocument.CopyToAsync(stream);
        var userDocBytes = stream.ToArray();

        // ── Run the validation pipeline ───────────────────────────────────────
        var result = await _orchestrator.ValidateAsync(
            userDocBytes,
            request.UserDocument.ContentType,
            documentName,
            templateBytes,
            templateMediaType,
            rules);

        return Ok(result);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>Splits a multi-line / semicolon-delimited rule string into individual rules.</summary>
    private static List<string> ParseRules(string? generalRule)
    {
        if (string.IsNullOrWhiteSpace(generalRule))
            return [];

        return generalRule
            .Split(['\n', '\r', ';'], StringSplitOptions.RemoveEmptyEntries)
            .Select(r => r.Trim())
            .Where(r => r.Length > 0)
            .ToList();
    }

    /// <summary>Resolves a media type from a file extension.</summary>
    private static string ResolveMediaType(string filePath) =>
        Path.GetExtension(filePath).ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png"            => "image/png",
            ".gif"            => "image/gif",
            ".webp"           => "image/webp",
            ".pdf"            => "application/pdf",
            _                 => "image/jpeg"
        };
}