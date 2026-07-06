using Khedmetak.DAL.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.DAL.Entities
{
    public class GovService : BaseEntity
    {
        public string SrvName { get; set; } = string.Empty;
        public string SrvDesc { get; set; } = string.Empty;
        public decimal SrvFees { get; set; }
        public string SrvTime { get; set; } = string.Empty;
        public decimal EstimatedFees { get; set; }

        // بيانات إضافية تظهر أعلى صفحة الخدمة (الشريط العلوي)
        public string ProviderEntity { get; set; } = string.Empty;   // الجهة المقدمة للخدمة
        public string TargetAudience { get; set; } = string.Empty;   // الفئة المستهدفة
        public string DeliveryMethod { get; set; } = string.Empty;   // طريقة الاستلام
        public bool NeedsGuarantee { get; set; }                     // يحتاج ضمان؟

        #region Foreign Keys
        public int CategoryId { get; set; }
        #endregion

        #region Relations
        public Category Category { get; set; } = null!;
        public ICollection<ServiceSteps> ServiceSteps { get; set; } = new List<ServiceSteps>();
        public ICollection<ServiceGeneralDocs> ServiceGeneralDocs { get; set; } = new List<ServiceGeneralDocs>();
        public ICollection<RequiredDocument> RequiredDocuments { get; set; } = new List<RequiredDocument>();
        public ICollection<ServiceOption> ServiceOptions { get; set; } = new List<ServiceOption>();
        public ICollection<ServiceFeeTier> ServiceFeeTiers { get; set; } = new List<ServiceFeeTier>();
        public ICollection<ServiceImportantNote> ImportantNotes { get; set; } = new List<ServiceImportantNote>();

        #endregion
    }
}
