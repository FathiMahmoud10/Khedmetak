using Khedmetak.DAL.Entities.Base;
using Khedmetak.DAL.Enums;
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
        public Guid SessionGuid { get; set; } = Guid.NewGuid();

        // Status of the service request represented by this session (Pending/InProgress/Completed/Rejected)
        public RequestStatus Status { get; set; } = RequestStatus.Pending;

        #region Foreign Keys
        public int? UserId { get; set; } // make it nullable
        public int? CategoryId { get; set; }
        public int? GovServiceId { get; set; } // the gov service this session/request is linked to
        #endregion

        #region Relations
        public User? User { get; set; } = null!; // make it nullable
        public Category? Category { get; set; }
        public GovService? GovService { get; set; }
        public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
        public ICollection<UserDocument> UserDocuments { get; set; } = new List<UserDocument>();
        public Feedback? Feedback { get; set; }
        #endregion
    }
}
