// Khedmetak.Core/Entities/User.cs

using Microsoft.AspNetCore.Identity;

namespace Khedmetak.Core.Entities;

public class User : IdentityUser
{
    public string Name { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    #region Relations

    public ICollection<ChatSession> ChatSessions { get; set; } = new List<ChatSession>();


    #endregion

}