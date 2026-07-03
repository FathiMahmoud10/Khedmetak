using Khedmetak.AI.Configuration;
using Khedmetak.AI.DTOs;
using Khedmetak.AI.DTOs.RagDTOs;
using Khedmetak.AI.Services.Abstraction;
using Microsoft.Extensions.Options;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using System.Security.Cryptography;
using System.Text;



namespace Khedmetak.AI.Services.Implementation
{

    public class QdrantService : IVectorDB
    {

        private string CollectionName;
        private readonly QdrantClient _client;

        public QdrantService(QdrantClient client, IOptions<QdrantDBSettings> options)
        {

            CollectionName = options.Value.QdrantCollection;

            _client = client;
        }

        // ========================= UPSERT CHUNKS =========================
        // ================ Add Service chunks and their embedding content of each chunk to vectorDatabase ===================
        public async Task UpsertServiceChunksAsync(List<ServiceChunkDTO> chunks, Func<string, Task<float[]>> embedFunc)
        {
            var points = new List<PointStruct>();

            foreach (var chunk in chunks)
            {
                float[] vector = await embedFunc(chunk.Content);

                var point = new PointStruct
                {
                    Id = new PointId
                    {
                        Uuid = CreateDeterministicGuid(chunk.ChunkId)
                    },
                    Vectors = new Dictionary<string, Vector>
                    {
                        ["dense"] = vector
                    },

                    Payload =
                    {
                        ["ServiceId"] = chunk.ServiceId,
                        ["ServiceName"] = chunk.ServiceName,
                        ["CategoryId"] = chunk.CategoryId,
                        ["CategoryName"] = chunk.CategoryName ?? "",

                        ["ChunckId"] = chunk.ChunkId,
                        ["ChunckType"] = chunk.ChunkType,

                        ["Content"] = chunk.Content,
                        ["Language"] = "ar"
                    }
                };

                points.Add(point);
            }

            await _client.UpsertAsync(
                collectionName: CollectionName,
                points: points
            );
        }



        public async Task UpsertServiceChunkAsync(ServiceChunkDTO chunk, Func<string, Task<float[]>> embedFunc)
        {
            
                float[] vector = await embedFunc(chunk.Content);

                var point = new PointStruct
                {
                    Id = new PointId
                    {
                        Uuid = CreateDeterministicGuid(chunk.ChunkId)
                    },
                    Vectors = new Dictionary<string, Vector>
                    {
                        ["dense"] = vector
                    },

                    Payload =
                    {
                        ["ServiceId"] = chunk.ServiceId,
                        ["ServiceName"] = chunk.ServiceName,
                        ["CategoryId"] = chunk.CategoryId,
                        ["CategoryName"] = chunk.CategoryName ?? "",

                        ["ChunckId"] = chunk.ChunkId,
                        ["ChunckType"] = chunk.ChunkType,

                        ["Content"] = chunk.Content,
                        ["Language"] = "ar"
                    }
                };

             
            await _client.UpsertAsync(
                collectionName: CollectionName,
                points: new[] { point }
            );
        }


        // ========================= DELETE ALL CHUNKS OF SERVICE =========================
        public async Task DeleteServiceChunksAsync(int serviceId)
        {
            await _client.DeleteAsync(
                collectionName: CollectionName,
                filter: new Filter
                {
                    Must =
                    {
                        new Condition
                        {
                            Field = new FieldCondition
                            {
                                Key = "ServiceId",
                                Match = new Match
                                {
                                    Integer = serviceId
                                }
                            }
                        }
                    }
                });
        }

        // ========================= Search About Relevant Chunks to embedding of user question  in Vector DB =========================

        public async Task<IReadOnlyList<ScoredPoint>> SearchInVectorDBAsync(float[] userQuestionEmbedding)
        {
             var results = await _client.QueryAsync(
               collectionName: CollectionName,
               query: userQuestionEmbedding,   // float[] / ReadOnlyMemory<float> etc.
               usingVector: "dense",           // name of the named vector
               limit: 10
           );
            return results;
        }
        // ========================= Search About the moast match Chunk to embedding of user question  in Vector DB =========================

        public async Task<RagServiceInfo?> SearchServiceAsync(float[] userQuestionEmbedding)
        {
            var results = await _client.QueryAsync(
                collectionName: CollectionName,
                query: userQuestionEmbedding,
                usingVector: "dense",
                limit: 3
            );

            var point = results.FirstOrDefault(p => p.Score >= 0.4f);

            if (point == null)
                return null;

            if (!point.Payload.TryGetValue("ServiceId", out var serviceIdValue) ||
                !point.Payload.TryGetValue("ServiceName", out var serviceNameValue))
            {
                return null;
            }

            return new RagServiceInfo
            {
                ServiceId = (int)serviceIdValue.IntegerValue,
                ServiceName = serviceNameValue.StringValue
            };
        }

        // =========== to Generate the Point Id when add chunk to  Vector Database  ==========
        private static string CreateDeterministicGuid(string value)
        {
            using var md5 = MD5.Create();

            byte[] hash = md5.ComputeHash(
                Encoding.UTF8.GetBytes(value)
            );

            return new Guid(hash).ToString();
        }
    }
}
