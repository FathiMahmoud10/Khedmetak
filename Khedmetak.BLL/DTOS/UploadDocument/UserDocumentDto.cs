// Khedmetak.BLL/DTOS/Documents/UserDocumentDto.cs
namespace Khedmetak.BLL.DTOS.Documents
{
    public class UserDocumentDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? RequiredDocumentId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }
}