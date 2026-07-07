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
        public string DocumentModel { get; set; } = "meta/Llama-4-Scout-17B-16E-Instruct";
        public string EmbeddingModel { get; set; } = "jina-embeddings-v5-text-small";
        public string ApiKey { get; set; } = string.Empty;
        public int MaxToken { get; set; } = 500;
       

    }
}
