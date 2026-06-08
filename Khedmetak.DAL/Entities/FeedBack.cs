using Khedmetak.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.DAL.Entities
{
    public class Feedback
    {
        public int FeedBackId { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Comments { get; set; }
        public int ChatSessionId { get; set; }
        public ChatSession ChatSession { get; set; } = null!;

    }
}
