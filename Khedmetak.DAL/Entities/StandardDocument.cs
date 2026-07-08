using Khedmetak.DAL.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.DAL.Entities
{
    public class StandardDocument : BaseEntity
    {
        public string DocumentName { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string? GeneralRule { get; set; }

        #region Foreign Keys
        public int RequiredDocumentId { get; set; }
        #endregion

        #region Relations
        public RequiredDocument RequiredDocument { get; set; } = null!;
        #endregion
    }
}
