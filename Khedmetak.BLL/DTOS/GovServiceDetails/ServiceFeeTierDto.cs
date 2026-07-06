using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.BLL.DTOS.GovServiceDetails
{
    public class ServiceFeeTierDto
    {
        public int Id { get; set; }
        public string TierName { get; set; } = string.Empty;
        public decimal Fees { get; set; }
        public string Duration { get; set; } = string.Empty;
        public bool IsRefundable { get; set; }
    }
}
