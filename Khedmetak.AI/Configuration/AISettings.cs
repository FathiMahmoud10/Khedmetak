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
        public string Model { get; set; } = "openai/gpt-4o";
        public string DocumentModel { get; set; } = "openai/gpt-4.1";
        public string EmbeddingModel { get; set; } = "jina-embeddings-v5-text-small";
        public string ApiKey { get; set; } = string.Empty;
        public int MaxToken { get; set; } = 500;
       

    }
}
