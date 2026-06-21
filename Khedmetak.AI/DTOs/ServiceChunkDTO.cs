using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.DTOs
{
    public   enum  ChunkType
    {
        Overview, RequiredDocuments, Steps,Fees
    }
    public class ServiceChunkDTO
    {
        public string ChunkId { get; set; } = string.Empty;
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string? CategoryName { get; set; }
        public int CategoryId { get; set; }
        public string ChunkType { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        // Metadata
        //public Dictionary<string, object> Metadata { get; set; } = new();
    }
}
