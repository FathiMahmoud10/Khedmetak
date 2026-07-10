using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.DigitalPortal.DTOs
{
    public class DigitalPortalOtpDto
    {
        public string NationalId { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
    }

}
