// Khedmetak.Core/Entities/User.cs

using Microsoft.AspNetCore.Identity;

namespace Khedmetak.Core.Entities;

public class User : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string PreferredLanguage { get; set; } = "ar";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Document> Documents { get; set; } = new List<Document>();
    public ICollection<ChatSession> ChatSessions { get; set; } = new List<ChatSession>();
}