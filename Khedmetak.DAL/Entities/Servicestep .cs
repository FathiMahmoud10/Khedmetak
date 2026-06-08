using Khedmetak.DAL.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.DAL.Entities
{
    public class Servicestep : BaseEntity
    {
        public string Title { get; set; } = string.Empty;

        public int StepOrder { get; set; }

        public int GovServiceId { get; set; }
        public GovService GovService { get; set; } = null!;
    }
}
