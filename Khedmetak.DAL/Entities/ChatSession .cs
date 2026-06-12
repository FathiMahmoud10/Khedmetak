using Khedmetak.DAL.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.DAL.Entities
{
    public class ChatSession : BaseEntity
    {
        public DateTime StartedAt { get; set; } = DateTime.Now; // add initial date to it
        public DateTime? EndedAt { get; set; }

        #region Foreign Keys
        public int? UserId { get; set; } // make it nullable
        public int? CategoryId { get; set; }
        #endregion

        #region Relations
        public User? User { get; set; } = null!; // make it nullable
        public Category? Category { get; set; }
        public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
        public ICollection<UserDocument> UserDocuments { get; set; } = new List<UserDocument>();
        public Feedback? Feedback { get; set; }
        #endregion
    }
}
