using Khedmetak.DAL.Entities.Base;
using Khedmetak.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Khedmetak.DAL.Entities
{
    public class RequiredDocument : BaseEntity
    {
        public string DocumentName { get; set; } = string.Empty;
        public bool IsMandatory { get; set; }
        public DocumentType DocumentType { get; set; } = DocumentType.Any;

        #region Foreign Keys
        public int GovServiceId { get; set; }
        #endregion

        #region Relations
        public GovService GovService { get; set; } = null!;
        public ICollection<UserDocument> UserDocuments { get; set; } = new List<UserDocument>();
        #endregion
    }
}
