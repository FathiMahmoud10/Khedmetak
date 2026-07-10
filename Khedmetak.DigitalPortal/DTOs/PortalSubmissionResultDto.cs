using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.DigitalPortal.DTOs
{
    public class PortalSubmissionResultDto
    {
        // وظيفة هذا الكلاس هي تمثيل نتيجة عملية الإرسال والإصدار من البوابة الرقمية،
        // حيث يحتوي على معلومات حول نجاح العملية،
        // رسالة توضيحية، معرف المعاملة،
        // والمستند الرسمي الصادر من البوابة الرقمية (إن وجد).
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? TransactionId { get; set; }
        public IssuedDocumentDto? IssuedDocument { get; set; }
    }
}
