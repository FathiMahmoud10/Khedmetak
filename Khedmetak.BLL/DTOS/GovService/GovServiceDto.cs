using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.BLL.DTOS.GovService
{
    public class GovServiceDto
    {
        public int Id { get; set; }
        public string SrvName { get; set; } = string.Empty;
        public string SrvDesc { get; set; } = string.Empty;
        public decimal SrvFees { get; set; }
        public string SrvTime { get; set; } = string.Empty;
        public decimal EstimatedFees { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
    }
}
