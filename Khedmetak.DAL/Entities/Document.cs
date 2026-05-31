// Khedmetak.Core/Entities/Document.cs

namespace Khedmetak.Core.Entities;

public class Document
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public bool IsVerified { get; set; } = false;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public string UserId { get; set; } = string.Empty;
    public User User { get; set; } = null!;

    public int GovernmentServiceId { get; set; }
    public GovernmentService GovernmentService { get; set; } = null!;
}