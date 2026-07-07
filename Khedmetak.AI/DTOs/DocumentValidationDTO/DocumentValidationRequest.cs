using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.DTOs.DocumentValidationDTO
{
    public class DocumentValidationRequest
    {
        /// <summary>
        /// The primary document image to validate.
        /// </summary>
        public IFormFile Document { get; set; } = default!;

        /// <summary>
        /// Optional second image to compare against the primary document.
        /// Use cases: a selfie to face-match against an ID photo, or a second
        /// copy/angle of the same document to cross-check consistency.
        /// </summary>
        public IFormFile? ComparisonDocument { get; set; }

        /// <summary>
        /// Optional expected document type (e.g. "National ID", "Passport",
        /// "Birth Certificate"). If provided, the model checks whether the
        /// uploaded document actually matches this expected type.
        /// </summary>
        public string? ExpectedDocumentType { get; set; }

        /// <summary>
        /// Optional list of custom rules the document must satisfy, e.g.
        /// "Must not be expired", "Photo must be clearly visible",
        /// "Must show a national ID number". Each rule is evaluated
        /// individually and returned with a pass/fail verdict.
        /// Send as repeated form fields: Rules=rule1&Rules=rule2
        /// </summary>
        public List<string>? Rules { get; set; }
    }
}