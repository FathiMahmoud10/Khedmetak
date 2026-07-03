using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.DAL.Entities
{
    // Khedmetak.DAL.Entities
    public class Payment
    {
        public int Id { get; set; }
        public string MerchantRefNum { get; set; }      // الـ Reference بتاعنا
        public string? FawryRefNumber { get; set; }     // كود فوري (للدفع في الفرع)
        public string? PaymentUrl { get; set; }         // رابط الدفع بالكارت
        public string PaymentMethod { get; set; }       // PAYATFAWRY / CARD
        public decimal Amount { get; set; }
        public string Status { get; set; } = "PENDING"; // PENDING / PAID / UNPAID / EXPIRED
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PaidAt { get; set; }

        // Foreign Key للـ User
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
