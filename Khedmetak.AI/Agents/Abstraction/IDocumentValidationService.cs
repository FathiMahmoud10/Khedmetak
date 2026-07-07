using Khedmetak.AI.DTOs.DocumentValidationDTO;

namespace Khedmetak.AI.Agents.Abstraction
{
    public interface IDocumentValidationService
    {
        Task<DocumentValidationResult> ValidateAsync(
            byte[] imageBytes,
            string mediaType,
            byte[]? comparisonImageBytes = null,
            string? comparisonMediaType = null,
            string? expectedDocumentType = null,
            List<string>? rules = null);
    }
}