using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.Configuration
{
    public class AISettings
    {
        public string Provider { get; set; } = "GitHubModels";
        public string Model { get; set; } =  "meta/Llama-3.3-70B-Instruct";
        public string EmbeddingModel { get; set; } = "openai/text-embedding-3-small";
        public string ApiKey { get; set; } = string.Empty;
        public int MaxToken { get; set; } = 200;
        // Qdrant settings
        public string QdrantEndpoint { get; set; } = "http://localhost:6333";
        public string QdrantCollection { get; set; } = "khedmetak_services";
        public string? QdrantApiKey { get; set; } = null;

    }
}
