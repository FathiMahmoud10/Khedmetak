using System;

namespace Khedmetak.AI.DTOs.ChatSessionDTO
{
    public class ServiceRequestResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public Guid SessionGuid { get; set; }

        // ── بيانات إضافية ──
        public DateTime? SubmittedAt { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? PhoneNumber { get; set; }
        public int UploadedFilesCount { get; set; }
    }
}
