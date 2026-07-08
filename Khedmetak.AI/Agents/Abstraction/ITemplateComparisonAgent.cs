using System.Collections.Generic;
using System.Threading.Tasks;
using Khedmetak.AI.DTOs;

namespace Khedmetak.AI.Agents.Abstraction;

/// <summary>
/// Validates the uploaded document image against an optional template and a list of image rules.
/// Combines template comparison and image quality validation in one focused agent.
/// </summary>
public interface ITemplatesAgent
{
    /// <summary>
    /// Validates the user document image.
    /// </summary>
    /// <param name="userDocumentBytes">The uploaded document image bytes.</param>
    /// <param name="mediaType">MIME type of the uploaded document.</param>
    /// <param name="templateImageBytes">Optional official template image to compare layout against.</param>
    /// <param name="templateMediaType">MIME type of the template image.</param>
    /// <param name="expectedDocumentName">Expected document type name (e.g. "National ID").</param>
    /// <param name="imageRules">Visual/image-based rules to validate (e.g. "white background required").</param>
    Task<ImageValidationResult> ValidateAsync(
        byte[] userDocumentBytes,
        string mediaType,
        byte[]? templateImageBytes,
        string? templateMediaType,
        string expectedDocumentName,
        List<string> imageRules);
}
