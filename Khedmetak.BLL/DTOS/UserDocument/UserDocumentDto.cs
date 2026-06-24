namespace Khedmetak.BLL.DTOS.UserDocument
{
    public class UserDocumentDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public string ValidationStatus { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public int UserId { get; set; }
        public int? ChatSessionId { get; set; }
        public int? RequiredDocumentId { get; set; }
    }
}