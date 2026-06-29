using System;

namespace Khedmetak.AI.DTOs.ChatSessionDTO
{
    /// <summary>
    /// Response DTO returned after a successful chat file upload.
    /// </summary>
    public class ChatUploadResultDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public int? RequiredDocumentId { get; set; }
    }
}
