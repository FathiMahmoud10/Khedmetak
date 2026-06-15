using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.DTOs.ChatSessionDTO
{
    // to get all messages in a specific Session
    public class ChatSessionDTO 
    {
        public Guid SessionGuidId { get; set; }
        public List<ChatSessionMessageDTO>? ChatSession_ChatHistory { get; set; }
    }
}
