using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.DigitalPortal.DTOs
{
    public class PortalSubmissionRequestDto
    {
        // يُرسَل إلى البوابة الرقمية عند تقديم طلب المواطن لإصدار مستند رسمي.
        public string NationalId { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
    }

}
