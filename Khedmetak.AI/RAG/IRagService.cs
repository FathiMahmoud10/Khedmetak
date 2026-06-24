using Khedmetak.AI.DTOs.ChatSessionDTO;
using Qdrant.Client.Grpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.RAG
{
    public interface IRagService
    {
        public  Task<string> RagPipeline(string userQuestion);


       // Task<string> AskAsync(
       //string userQuestion,
       //ChatSessionDTO? chatSessionDto = null);

    }
}
