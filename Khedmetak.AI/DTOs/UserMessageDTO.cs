using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.DTOs
{
    public class UserMessageDTO
    {
        public string Message { get; set; }
        public int sessionId { get; set; } = -1;
    }
}
