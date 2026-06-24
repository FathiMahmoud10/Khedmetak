using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.DTOs.RagDTOs
{
    public class RagResponseDTO
    {
        public string Answer { get; set; } = string.Empty;

        public List<string> RetrievedChunks { get; set; } = new();
    }
}
