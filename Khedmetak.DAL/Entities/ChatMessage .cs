using Khedmetak.DAL.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.DAL.Entities
{
    public class ChatMessage : BaseEntity
    {
        public string Content { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }

        #region Foreign Keys
        public int ChatSessionId { get; set; }
        #endregion

        #region Relations
        public ChatSession ChatSession { get; set; } = null!;
        #endregion
    }
}
