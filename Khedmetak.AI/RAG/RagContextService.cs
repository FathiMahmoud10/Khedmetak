using Khedmetak.AI.Services.Abstraction;
using Khedmetak.AI.Services.Implementation;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.RAG
{
    public class RagContextService: IRagContextService
    {
        private readonly IVectorDBOperationsService vectorDBOperations;
        public RagContextService(IVectorDBOperationsService vectorDB)
        {
            this.vectorDBOperations = vectorDB;
        }

        // To Generate Context "combine Text" of the most Relative chunks  that are returned from Vector DB  to user question
        public async Task<string> GenerateContextFromQuestionAsync(string userQuestion)
        {
         // 1. take user question then embedding it to float[]
         // then search in vector db to relevant chunks and return them
            var results = await vectorDBOperations.SearchInVectorDBAsync(userQuestion);

            // 2. Extract Chunks and build context

            var chunks = results.Where(x => x.Score > 0.40)
                .Select(x =>x.Payload["Content"].StringValue)
                .ToList();

            var context = string.Join("\n\n", chunks);

            return context;

        } 


    }
}
