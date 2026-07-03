using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.BLL.DTOS.Fawry
{
    public class FawryStatusResponse
    {
        public string MerchantRefNum { get; set; }
        public string FawryRefNumber { get; set; }
        public decimal PaymentAmount { get; set; }
        public string PaymentStatus { get; set; }  // PAID / UNPAID / EXPIRED
        public string StatusCode { get; set; }
        public string StatusDescription { get; set; }
    }
}
