using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.DTOs.ChatSessionDTO
{
    //      ----- to get message in session 
    public class ChatSessionMessageDTO
    {
        public int Id { get; set; }
        public string Role { get; set; }
        public string Content { get; set; }
    }
}
