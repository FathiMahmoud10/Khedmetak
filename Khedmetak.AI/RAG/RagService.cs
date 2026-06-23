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
    public class RagService: IRagService
    {
        private readonly IEmbeddingService embeddingService;
        private readonly QdrantService qdrantService;

        public RagService(IEmbeddingService embeddingService, QdrantService qdrantService) {
            this.embeddingService = embeddingService;
            this.qdrantService = qdrantService;
        }

        public async Task<string> RagPipeline(string userQuestion)
        {
            // ----- 1- Convert User question to Embedding vector
            var userQuestionEmbedding = await embeddingService.GenerateEmbeddingAsync(userQuestion);

            // ------- 2- Search Qdrant ------
            // send Query to vectordatabase with user question embedding to retrieve relative embedding chunks of question

            var results = await qdrantService.SearchQudrant(userQuestionEmbedding);

            // 5. Extract Chunks and build context

            var chunks = results.Where(x => x.Score > 0.60)
                .Select(x =>
                    x.Payload["Content"].StringValue)
                .ToList();

            var context =
                string.Join("\n\n", chunks);

            return context;

        } 


    }
}
