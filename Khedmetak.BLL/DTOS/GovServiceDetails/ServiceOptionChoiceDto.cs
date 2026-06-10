using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.BLL.DTOS.GovServiceDetails
{
    public class ServiceOptionChoiceDto
    {
        public int Id { get; set; }
        public string Choice { get; set; } = string.Empty;
        public bool IsRequired { get; set; }
    }
}
