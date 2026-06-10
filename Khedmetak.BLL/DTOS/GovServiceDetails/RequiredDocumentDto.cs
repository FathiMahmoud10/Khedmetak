using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.BLL.DTOS.GovServiceDetails
{
    public class RequiredDocumentDto
    {
        public int Id { get; set; }
        public string DocumentName { get; set; } = string.Empty;
        public bool IsMandatory { get; set; }
    }
}
