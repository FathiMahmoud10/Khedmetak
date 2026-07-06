using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.BLL.DTOS.GovServiceDetails
{
    public class GovServiceDetailsDto
    {
        public int Id { get; set; }
        public string SrvName { get; set; } = string.Empty;
        public string SrvDesc { get; set; } = string.Empty;
        public decimal SrvFees { get; set; }
        public string SrvTime { get; set; } = string.Empty;
        public decimal EstimatedFees { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int CategoryId { get; set; }

        // بيانات الشريط العلوي بصفحة الخدمة
        public string ProviderEntity { get; set; } = string.Empty;
        public string TargetAudience { get; set; } = string.Empty;
        public string DeliveryMethod { get; set; } = string.Empty;
        public bool NeedsGuarantee { get; set; }

        public List<ServiceStepDto> Steps { get; set; } = new();
        public List<RequiredDocumentDto> RequiredDocuments { get; set; } = new();
        public List<ServiceOptionDto> Options { get; set; } = new();
        public List<ServiceGeneralDocDto> GeneralDocs { get; set; } = new();
        public List<ServiceFeeTierDto> FeeTiers { get; set; } = new();
        public List<ServiceImportantNoteDto> ImportantNotes { get; set; } = new();
    }
}
