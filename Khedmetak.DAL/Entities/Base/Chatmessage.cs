using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.DAL.Entities.Base
{
    public class ChatMessage : BaseEntity
    {
        public string Content { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public DateTime? StartedAt { get; set; }

        public DateTime? EndedAt { get; set; }

        #region Relation
        /*العلاقه بين الرساله و رقم الشات */

        public int ChatSessionId { get; set; }


        public ChatSession ChatSession { get; set; } = null!;
        #endregion

    }
}
