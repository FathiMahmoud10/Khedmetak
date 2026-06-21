using Khedmetak.AI.Configuration;
using Khedmetak.AI.DTOs;
using Khedmetak.AI.Services.Abstraction;
using Microsoft.Extensions.Options;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using System.Security.Cryptography;
using System.Text;


namespace Khedmetak.AI.Services.Implementation
{
    

    public class QdrantService: IQdrantService
    {
        

        private  string CollectionName;
        private readonly QdrantClient _client;

        public QdrantService(QdrantClient client, IOptions<QdrantDBSettings> options)
        {
           
            CollectionName = options.Value.QdrantCollection;

            _client = client;
        }
        
        // =========================
        // UPSERT CHUNKS
        // =========================

        // ================ Add Service chunks to vectorDatabase and make  embedding for each chunk content ===================
        public async Task UpsertServiceChunksAsync( List<ServiceChunkDTO> chunks,Func<string, Task<float[]>> embedFunc)
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

                        Vectors = vector,

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


        // =========================
        //DELETE ALL CHUNKS OF SERVICE
        //(for updates)
        //=========================
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
