using Khedmetak.DAL.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.DAL.Entities
{
    // يمثل صف واحد في جدول "الرسوم والتكاليف" بصفحة الخدمة
    // (عادية / عاجلة / خاصة / VIP / فورية ... إلخ)
    public class ServiceFeeTier : BaseEntity
    {
        public string TierName { get; set; } = string.Empty;   // اسم نوع الاستمارة: عادية، عاجلة، خاصة، VIP، فورية
        public decimal Fees { get; set; }                       // رسوم هذا النوع
        public string Duration { get; set; } = string.Empty;   // المدة المطلوبة للاستلام لهذا النوع
        public bool IsRefundable { get; set; }                 // هل الرسوم قابلة للاسترداد
        public int DisplayOrder { get; set; }                  // ترتيب العرض في الصفحة

        #region Foreign Keys
        public int GovServiceId { get; set; }
        #endregion

        #region Relations
        public GovService GovService { get; set; } = null!;
        #endregion
    }
}
