using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Khedmetak.BLL.DTOS.UserDocument
{
    public class UploadDocumentDto
    {
        [Required(ErrorMessage = "الملف مطلوب")]
        public IFormFile File { get; set; } = null!;

        public int? ChatSessionId { get; set; }
        public int? RequiredDocumentId { get; set; }
    }
}