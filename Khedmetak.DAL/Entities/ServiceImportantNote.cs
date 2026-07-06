using Khedmetak.DAL.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.DAL.Entities
{
    // يمثل نقطة واحدة في شريط "معلومات مهمة" أسفل صفحة الخدمة
    public class ServiceImportantNote : BaseEntity
    {
        public string Note { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }

        #region Foreign Keys
        public int GovServiceId { get; set; }
        #endregion

        #region Relations
        public GovService GovService { get; set; } = null!;
        #endregion
    }
}
