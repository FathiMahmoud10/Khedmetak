using Khedmetak.DAL.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.DAL.Entities
{
    public class Feedback : BaseEntity
    {
        public int Rating { get; set; }
        public string Comments { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        #region Foreign Keys
        public int UserId { get; set; }
        public int ChatSessionId { get; set; }
        #endregion

        #region Relations
        public User User { get; set; } = null!;
        public ChatSession ChatSession { get; set; } = null!;
        #endregion
    }
}
