// Khedmetak.Core/Entities/ChatSession.cs

using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Entities.Base;

namespace Khedmetak.Core.Entities;

public class ChatSession : BaseEntity
{
    public int? UserId { get; set; }
    
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EndedAt { get; set; }

    #region Relations
    public User? User { get; set; } = null!;
    public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public Feedback? Feedback { get; set; }

    #endregion

}