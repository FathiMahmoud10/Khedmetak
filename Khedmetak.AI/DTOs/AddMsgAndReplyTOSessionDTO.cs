using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.DTOs
{
    // to add and save the user message and AI Response of a specific session together to Database
    public class AddMsgAndReplyTOSessionDTO
    {
        public int SessionId { get; set; }
        public string UserMessage { get; set; }
        public string AIResponse { get; set; }

    }
}
