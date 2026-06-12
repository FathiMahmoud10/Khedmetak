using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.Configuration
{
    public class AISettings
    {
        public string Provider { get; set; } = "OpenRouter";
        public string Model { get; set; } = "meta-llama/llama-3.3-70b-instruct";
        public string ApiKey { get; set; }=string.Empty;


    }
}
