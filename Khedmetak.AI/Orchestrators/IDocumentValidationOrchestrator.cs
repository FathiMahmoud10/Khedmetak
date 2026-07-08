using System.Collections.Generic;
using System.Threading.Tasks;
using Khedmetak.AI.DTOs.DocumentValidationDTO;

namespace Khedmetak.AI.Orchestrators;

public interface IDocumentValidationOrchestrator
{
    /// <summary>
    /// Runs the full validation pipeline for a user-uploaded document.
    /// </summary>
    /// <param name="userDocumentBytes">Uploaded document image bytes.</param>
    /// <param name="mediaType">MIME type of the uploaded document.</param>
    /// <param name="expectedDocumentName">Document name loaded from the database.</param>
    /// <param name="templateImageBytes">Optional official template image loaded from the database.</param>
    /// <param name="templateMediaType">MIME type of the template image.</param>
    /// <param name="rules">All validation rules loaded from the database.</param>
    Task<DocumentValidationResult> ValidateAsync(
        byte[] userDocumentBytes,
        string mediaType,
        string expectedDocumentName,
        byte[]? templateImageBytes,
        string? templateMediaType,
        List<string> rules);
}
