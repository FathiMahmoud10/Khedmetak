using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.DTOs.ChatSessionDTO
{

    public class NewSessionDTO
    {
        public string? UserEmail {  get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
