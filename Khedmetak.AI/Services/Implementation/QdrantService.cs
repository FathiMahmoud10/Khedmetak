using Grpc.Net.Client;
using Khedmetak.AI.DTOs;
using Khedmetak.AI.Services.Abstraction;
using Khedmetak.DAL.Entities;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.Services.Implementation
{
    

    public class QdrantService: IQdrantService
    {
        

        private const string CollectionName = "testCollection";
        private readonly QdrantClient _client;

        public QdrantService()
        {
            _client = new QdrantClient(
                host: "f38256f1-5c7e-4ad7-9948-59b0d47c0aed.sa-east-1-0.aws.cloud.qdrant.io",
                port: 6334,
                https: true,
                apiKey: ""
            );
        }

        // =========================
        // UPSERT CHUNKS
        // =========================


        public async Task UpsertServiceChunksAsync(
            GovService service,
            List<ServiceChunkDTO> chunks,
            Func<string, Task<float[]>> embedFunc)
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
                        ["ServiceId"] = service.Id,
                        ["ServiceName"] = service.SrvName,
                        ["CategoryId"] = service.CategoryId,
                        ["CategoryName"] = service.Category?.Name ?? "",

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

        private static string CreateDeterministicGuid(string value)
        {
            using var md5 = MD5.Create();

            byte[] hash = md5.ComputeHash(
                Encoding.UTF8.GetBytes(value)
            );

            return new Guid(hash).ToString();
        }
        // =========================
        // DELETE ALL CHUNKS OF SERVICE
        // (for updates)
        // =========================
        //    public async Task DeleteServiceChunksAsync(int serviceId)
        //    {
        //        await _client.DeleteAsync(CollectionName,
        //            filter: new Filter
        //            {
        //                Must =
        //                {
        //                new Condition
        //                {
        //                    Field = "serviceId",
        //                    Match = new MatchValue { Value = serviceId }
        //                }
        //                }
        //            });
        //    }
    }
}
