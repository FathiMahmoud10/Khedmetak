using Khedmetak.AI.DTOs;
using Qdrant.Client.Grpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.Services.Abstraction
{
    public interface IVectorDB
    {
        public Task UpsertServiceChunksAsync(List<ServiceChunkDTO> chunks, Func<string, Task<float[]>> embedFunc);

        public Task DeleteServiceChunksAsync(int serviceId);
        public Task<IReadOnlyList<ScoredPoint>> SearchInVectorDBAsync(float[] userQuestionEmbedding);
    }
}
