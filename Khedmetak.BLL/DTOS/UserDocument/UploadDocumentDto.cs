using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Khedmetak.BLL.DTOS.UserDocument
{
    public class UploadDocumentDto
    {
        [Required(ErrorMessage = "الملف مطلوب")]
        public IFormFile File { get; set; } = null!;

        public int? ChatSessionId { get; set; }

        // FIX: the chat page only ever has the session's Guid (SessionGuidId) — the
        // numeric ChatSessionId is never returned to the frontend by /Session/newSession.
        // Accept the Guid here too so the service can resolve it to the real ChatSessionId
        // server-side instead of silently receiving null and never linking the file.
        public Guid? SessionGuidId { get; set; }

        public int? RequiredDocumentId { get; set; }
    }
}