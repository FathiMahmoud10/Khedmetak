using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.BLL.DTOS.Fawry
{
    public class FawryChargeRequest
    {
        public string CustomerProfileId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerMobile { get; set; }
        public string PaymentMethod { get; set; } // "PAYATFAWRY" أو "CARD"
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public List<FawryItem> Items { get; set; }
    }
}
