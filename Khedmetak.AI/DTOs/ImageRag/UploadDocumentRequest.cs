using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Khedmetak.AI.DTOs.ImageRag
{
    public class UploadDocumentRequest
    {
        [Required(ErrorMessage = "Image is required.")]
        public IFormFile Image { get; set; } = null!;

        [Required(ErrorMessage = "DocumentName is required.")]
        public string DocumentName { get; set; } = null!;
    }
}
