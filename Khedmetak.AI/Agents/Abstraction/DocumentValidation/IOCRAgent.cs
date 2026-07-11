using System.Threading.Tasks;
using Khedmetak.AI.DTOs;

namespace Khedmetak.AI.Agents.Abstraction.DocumentValidation;

public interface IOCRAgent
{
    Task<OCRResult> ExtractTextAsync(byte[] imageBytes, string mediaType);
}
