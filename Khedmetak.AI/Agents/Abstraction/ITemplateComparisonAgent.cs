using System.Threading.Tasks;
using Khedmetak.AI.DTOs;

namespace Khedmetak.AI.Agents.Abstraction;

public interface ITemplateComparisonAgent
{
    Task<TemplateComparisonResult> CompareAsync(
        byte[] imageBytes,
        string mediaType,
        byte[]? comparisonImageBytes = null,
        string? comparisonMediaType = null,
        string? expectedDocumentType = null);
}
