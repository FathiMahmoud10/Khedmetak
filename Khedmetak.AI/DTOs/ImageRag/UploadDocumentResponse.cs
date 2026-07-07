namespace Khedmetak.AI.DTOs.ImageRag
{
    public class UploadDocumentResponse
    {
        public bool Success { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
