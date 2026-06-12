using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.DTOs.ChatSessionDTO
{
    public class ChatSessionDTO
    {
        public int SessionId { get; set; }
        public List<ChatSessionMessageDTO>? SessionChatHistory { get; set; }
    }
}
