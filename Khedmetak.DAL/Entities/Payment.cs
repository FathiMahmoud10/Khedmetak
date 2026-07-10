using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.DAL.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public string MerchantRefNum { get; set; }     
        public string? FawryRefNumber { get; set; }    
        public string? PaymentUrl { get; set; }        
        public string PaymentMethod { get; set; }       
        public decimal Amount { get; set; }
        public string Status { get; set; } = "PENDING";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PaidAt { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
