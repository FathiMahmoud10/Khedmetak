using Microsoft.AspNetCore.Http;

namespace Khedmetak.AI.DTOs.ImageRag
{
    public class UpdateDocumentRequest
    {
        public IFormFile? Image { get; set; }
        public string? DocumentName { get; set; }
    }
}
