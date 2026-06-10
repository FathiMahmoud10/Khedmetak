using Khedmetak.DAL.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.DAL.Entities
{
    public class ServiceOptionChoices : BaseEntity
    {
        public string Choice { get; set; } = string.Empty;
        public bool IsRequired { get; set; }

        #region Foreign Keys
        public int ServiceOptionId { get; set; }
        #endregion

        #region Relations
        public ServiceOption ServiceOption { get; set; } = null!;
        #endregion
    }
}
