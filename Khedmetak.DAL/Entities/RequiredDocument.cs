using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.DAL.Entities
{
    public class RequiredDocument
    {
        public int DocumentId { get; set; }

        public string DocumentName { get; set; } = string.Empty;

        public bool IsMandatory { get; set; }


        public int GovServiceId { get; set; }

        #region Relations
     public GovService GovService { get; set; } = null!;

        public ICollection<UserDocument> UserDocuments { get; set; } = new List<UserDocument>();

        #endregion
       }
}
