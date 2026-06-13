using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.DTOs
{
    // to send Reply from AI to a specefic session
    public class ChatResponseDTO
    {
        public int SessionId { get; set; }
        public string Message { get; set; }
        
    }
}
