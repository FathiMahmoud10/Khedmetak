using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.Configuration
{
    public class QdrantDBSettings
    {
        // Qdrant settings
        public string QdrantEndpoint { get; set; } = "f38256f1-5c7e-4ad7-9948-59b0d47c0aed.sa-east-1-0.aws.cloud.qdrant.io";
        public string QdrantCollection { get; set; } = "KhedmetakCollection";
        public string? QdrantApiKey { get; set; } = null;
    }
}
