using Khedmetak.DAL.Entities.Base;
using Khedmetak.DAL.Entities.Khedmetak.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.DAL.Entities
{
    public class User : IdentityUser<int>
    {
        public string Name { get; set; } = string.Empty;
     
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        #region Relations
        public ICollection<ChatSession> ChatSessions { get; set; } = new List<ChatSession>();
        public ICollection<UserDocument> UserDocuments { get; set; } = new List<UserDocument>();
        public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
        public CitizenProfile? CitizenProfile { get; set; }
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();

        #endregion
    }
}