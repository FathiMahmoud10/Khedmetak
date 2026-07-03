using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.BLL.DTOS.Fawry
{
    public class FawryChargeResponse
    {
        public int Type { get; set; }
        public string ReferenceNumber { get; set; } // كود فوري (للدفع في الفرع)
        public string MerchantRefNum { get; set; }
        public decimal PaymentAmount { get; set; }
        public string PaymentUrl { get; set; }  // رابط الدفع بالكارت
        public string StatusCode { get; set; }
        public string StatusDescription { get; set; }
    }
}
