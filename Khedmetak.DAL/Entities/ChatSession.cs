// Khedmetak.Core/Entities/ChatSession.cs

namespace Khedmetak.Core.Entities;

public class ChatSession
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Response { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    public string UserId { get; set; } = string.Empty;
    public User User { get; set; } = null!;

    public int GovernmentServiceId { get; set; }
    public GovernmentService GovernmentService { get; set; } = null!;
}