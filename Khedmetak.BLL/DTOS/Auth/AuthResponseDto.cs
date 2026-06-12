using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.BLL.DTOS.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public IList<string> Roles { get; set; } = new List<string>();
        public DateTime ExpiresAt { get; set; }
    }
}
