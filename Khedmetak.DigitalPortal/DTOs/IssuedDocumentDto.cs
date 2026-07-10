using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.DigitalPortal.DTOs
{
    public class IssuedDocumentDto
    {
        // اصل المستند الرسمي من البوابة الرقمية (Base64).
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public string FileBase64 { get; set; } = string.Empty;
    }
}
