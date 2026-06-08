
using System.Reflection.Metadata;

namespace Khedmetak.Core.Entities;

public class GovernmentService
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Fees { get; set; }  // مصاريف
    public int EstimatedDays { get; set; } //الأيام المقدرة

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    #region Relations
    public ICollection<Document> Documents { get; set; } = new List<Document>();
    public ICollection<ChatSession> ChatSessions { get; set; } = new List<ChatSession>();
    #endregion
}