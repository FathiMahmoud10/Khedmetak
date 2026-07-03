using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.DAL.Entities.FawrySettings
{
    public class FawrySettings
    {
        public string MerchantCode { get; set; }
        public string SecurityKey { get; set; }
        public string BaseUrl { get; set; } = "https://atfawry.fawrystaging.com";
        public string ReturnUrl { get; set; }
        public string CallbackUrl { get; set; }
    }
}
