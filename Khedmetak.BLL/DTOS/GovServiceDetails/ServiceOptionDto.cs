using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.BLL.DTOS.GovServiceDetails
{
    public class ServiceOptionDto
    {
        public int Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public List<ServiceOptionChoiceDto> Choices { get; set; } = new();
    }
}
