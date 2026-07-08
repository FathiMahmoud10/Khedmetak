using System.Threading.Tasks;
using Khedmetak.AI.DTOs;

namespace Khedmetak.AI.Agents.Abstraction;

public interface IOCRAgent
{
    Task<OCRResult> ExtractTextAsync(byte[] imageBytes, string mediaType);
}
