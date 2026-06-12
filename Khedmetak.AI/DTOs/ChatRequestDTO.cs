using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.DTOs
{
    public class ChatRequestDTO
    {
        public string SessionId { get; set; }
        public string Message { get; set; }
        public string? SystemPrompt { get; set; }
        public float Temperature { get; set; }
        public List<ChatMessageDTO> ChatHistory = new();

    }
}
