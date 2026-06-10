using Khedmetak.DAL.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.DAL.Entities
{
    public class ServiceGeneralDocs : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public DateTime LastUpdated { get; set; }

        #region Foreign Keys
        public int GovServiceId { get; set; }
        #endregion

        #region Relations
        public GovService GovService { get; set; } = null!;
        #endregion
    }
}
