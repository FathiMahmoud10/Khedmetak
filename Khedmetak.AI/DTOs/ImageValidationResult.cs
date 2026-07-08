using System.Collections.Generic;

namespace Khedmetak.AI.DTOs;

/// <summary>
/// Minimal result returned by the TemplatesAgent containing only what the orchestrator needs.
/// </summary>
public class ImageValidationResult
{
    /// <summary>True when image quality and all image rules pass.</summary>
    public bool IsValid { get; set; }

    /// <summary>Document type detected in the image.</summary>
    public string DetectedDocumentType { get; set; } = "";

    /// <summary>Image rules that failed (subset of the rules passed in).</summary>
    public List<string> FailedImageRules { get; set; } = [];

    /// <summary>Human-readable messages describing why the image failed.</summary>
    public List<string> ValidationMessages { get; set; } = [];
}
