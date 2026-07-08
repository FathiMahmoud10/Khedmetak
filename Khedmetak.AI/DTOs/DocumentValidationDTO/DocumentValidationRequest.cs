using Microsoft.AspNetCore.Http;

namespace Khedmetak.AI.DTOs.DocumentValidationDTO
{
    /// <summary>
    /// Minimal multipart/form-data request accepted by the validation endpoint.
    /// All document metadata (name, template image, rules) is loaded server-side
    /// from the database using RequiredDocumentId.
    /// </summary>
    public class DocumentValidationRequest
    {
        /// <summary>The document image the user wishes to validate.</summary>
        public IFormFile UserDocument { get; set; } = default!;

        /// <summary>
        /// ID of the RequiredDocument entity that defines which document type is
        /// expected, its template image, and its validation rules.
        /// </summary>
        public int RequiredDocumentId { get; set; }
    }
}