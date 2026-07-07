using System;

namespace Khedmetak.AI.DTOs.ImageRag
{
    public class DocumentResponse
    {
        public string DocumentName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
