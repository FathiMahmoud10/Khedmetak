using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Khedmetak.AI.DTOs.ChatSessionDTO
{
    /// <summary>
    /// DTO for uploading a file from the chat page.
    /// Works with SessionGuidId so both guests and logged-in users can upload.
    /// </summary>
    public class ChatUploadDto
    {
        [Required(ErrorMessage = "الملف مطلوب")]
        public IFormFile File { get; set; } = null!;

        [Required(ErrorMessage = "معرف الجلسة مطلوب")]
        public Guid SessionGuidId { get; set; }

        public int? RequiredDocumentId { get; set; }
    }
}
