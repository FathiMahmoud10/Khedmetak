using Khedmetak.DAL.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.DAL.Entities
{
   
    public class ServiceFeeTier : BaseEntity
    {
        // اسم نوع الاستمارة: عادية، عاجلة، خاصة، VIP، فورية
        public string TierName { get; set; } = string.Empty; 
        // رسوم هذا النوع
        public decimal Fees { get; set; }                       
        // المدة المطلوبة للاستلام لهذا النوع
        public string Duration { get; set; } = string.Empty;  
        // هل الرسوم قابلة للاسترداد
        public bool IsRefundable { get; set; }                
        // ترتيب العرض في الصفحة
        public int DisplayOrder { get; set; }               

        #region Foreign Keys
        public int GovServiceId { get; set; }
        #endregion

        #region Relations
        public GovService GovService { get; set; } = null!;
        #endregion
    }
}
