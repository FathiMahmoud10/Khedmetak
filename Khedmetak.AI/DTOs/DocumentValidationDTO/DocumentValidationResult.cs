using System.Collections.Generic;

namespace Khedmetak.AI.DTOs.DocumentValidationDTO
{
    /// <summary>
    /// The single, clean validation response returned to the API client.
    /// Assembled exclusively by DocumentValidatorOrchestrator.
    /// </summary>
    public class DocumentValidationResult
    {
        /// <summary>True when the document passes all validation checks.</summary>
        public bool IsValid { get; set; }

        /// <summary>Detected document type (e.g. "National ID", "Passport").</summary>
        public string DocumentType { get; set; } = "";

        /// <summary>All validation errors merged from every agent. Empty when IsValid = true.</summary>
        public List<string> ValidationErrors { get; set; } = [];

        /// <summary>Key/value fields extracted via OCR. Null when OCR was not executed.</summary>
        public Dictionary<string, string>? ExtractedFields { get; set; }

        /// <summary>Optional non-blocking warnings (e.g. low confidence score).</summary>
        public List<string>? Warnings { get; set; }
    }
}