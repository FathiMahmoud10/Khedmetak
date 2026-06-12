using Khedmetak.DAL.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.DAL.Entities
{


        public class UserDocument : BaseEntity
        {
            public string FileName { get; set; } = string.Empty;   // ✅ was "Name"
            public string FilePath { get; set; } = string.Empty;
            public string FileType { get; set; } = string.Empty;   // ✅ added
            public DateTime UploadedAt { get; set; }               // ✅ added
            public string ValidationStatus { get; set; } = string.Empty;

            #region Foreign Keys
            public int UserId { get; set; }
            public int? ChatSessionId { get; set; }                // ✅ nullable — not every doc needs a chat
            public int? RequiredDocumentId { get; set; }
            #endregion

            #region Relations
            public User User { get; set; } = null!;
            public ChatSession? ChatSession { get; set; }          // ✅ nullable to match
            public RequiredDocument? RequiredDocument { get; set; }
            #endregion
        }
}
