using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.BLL.DTOS.GovServiceDetails
{
    public class ServiceStepDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int StepOrder { get; set; }
    }
}
