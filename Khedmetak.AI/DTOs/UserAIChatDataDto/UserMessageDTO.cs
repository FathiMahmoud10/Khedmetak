using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.DTOs.UserAIChatDataDto
{
    // to receive the user message for a specefic session 
    public class UserMessageDTO
    {
        public string Message { get; set; }
        public Guid SessionGuidId { get; set; } 
        
    }
}
