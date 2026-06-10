using Khedmetak.DAL.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.DAL.Entities
{
    public class ServiceOption : BaseEntity
    {
        public string Question { get; set; } = string.Empty;

        #region Foreign Keys
        public int GovServiceId { get; set; }
        #endregion

        #region Relations
        public GovService GovService { get; set; } = null!;
        public ICollection<ServiceOptionChoices> ServiceOptionChoices { get; set; } = new List<ServiceOptionChoices>();
        #endregion
    }
}
