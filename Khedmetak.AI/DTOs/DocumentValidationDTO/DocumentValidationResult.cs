using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.DTOs.DocumentValidationDTO
{
    public class DocumentValidationResult
    {
        public string DocumentType { get; set; } = "";
        public string Status { get; set; } = "";
        public float Confidence { get; set; }
        public bool Readable { get; set; }
        public bool AppearsAuthentic { get; set; }
        public bool PossibleManipulation { get; set; }

        public List<string> MissingInformation { get; set; } = [];
        public List<string> Issues { get; set; } = [];
        public List<string> SecurityFeaturesVisible { get; set; } = [];

        public Dictionary<string, string> ExtractedData { get; set; } = [];

        public string Summary { get; set; } = "";

        // --- Expected document type check (only set if ExpectedDocumentType was provided) ---
        public bool? MatchesExpectedType { get; set; }
        public string? ExpectedTypeNote { get; set; }

        // --- Comparison against a second image (only set if ComparisonDocument was provided) ---
        public bool? ComparisonPerformed { get; set; }
        public bool? ComparisonMatch { get; set; }
        public string? ComparisonSummary { get; set; }

        // --- Custom rule evaluation (empty if no Rules were provided) ---
        public List<RuleCheckResult> RuleResults { get; set; } = [];
    }

    public class RuleCheckResult
    {
        public string Rule { get; set; } = "";
        public bool Passed { get; set; }
        public string Note { get; set; } = "";
    }
}