using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.BLL.DTOS.Fawry
{
    public class FawryCallbackDto
    {
        public string RequestId { get; set; }
        public string FawryRefNumber { get; set; }
        public string MerchantRefNum { get; set; }
        public decimal PaymentAmount { get; set; }
        public string PaymentStatus { get; set; } // PAID, UNPAID, EXPIRED
        public string Signature { get; set; }
    }
}
