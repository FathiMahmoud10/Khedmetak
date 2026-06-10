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
        public string Name { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string ValidationStatus { get; set; } = string.Empty;

        #region Foreign Keys
        public int UserId { get; set; }
        public int ChatSessionId { get; set; }
        public int? RequiredDocumentId { get; set; }
        #endregion

        #region Relations
        public User User { get; set; } = null!;
        public ChatSession ChatSession { get; set; } = null!;
        public RequiredDocument? RequiredDocument { get; set; }
        #endregion
    }
}
