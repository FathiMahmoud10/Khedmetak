using System.Collections.Generic;
using System.Threading.Tasks;
using Khedmetak.AI.DTOs.DocumentValidationDTO;

namespace Khedmetak.AI.Orchestrators;

public interface IDocumentValidationOrchestrator
{
    Task<DocumentValidationResult> ValidateAsync(
        byte[] imageBytes,
        string mediaType,
        byte[]? comparisonImageBytes = null,
        string? comparisonMediaType = null,
        string? expectedDocumentType = null,
        List<string>? rules = null);
}
