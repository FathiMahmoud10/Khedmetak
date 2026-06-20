using Khedmetak.AI.DTOs;
using Khedmetak.AI.Services.Abstraction;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.shared;
using Khedmetak.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.Services.Implementation
{
    public class ChunkService :IChunkService
    {
        private readonly IGenericRepository<GovService> serviceRepository;

        public ChunkService(IGenericRepository<GovService> repo)
        {
            serviceRepository = repo;
        }


        public async Task<List<ServiceChunkDTO>> GenerateChunksAsync(int serviceId)
        {
            GovService? govService = await serviceRepository.GetByIdAsync(
                serviceId,
                s => s.Category,
                s => s.ServiceSteps,
                s => s.RequiredDocuments
            );

            if (govService == null)
                throw new Exception($"Service {serviceId} not found");

            var chunks = new List<ServiceChunkDTO>();

            // =========================
            // Helper for structured text
            // =========================
            string BuildBlock(params (string Label, string Value)[] fields)
            {
                return string.Join("\n",
                    fields
                        .Where(f => !string.IsNullOrWhiteSpace(f.Value))
                        .Select(f => $"{f.Label}: {f.Value}")
                );
            }

            //string header = $"خدمة حكومية: {govService.SrvName}\n";

            // =========================
            // Overview Chunk
            // =========================
            var overview = BuildBlock(
                ("اسم الخدمة", govService.SrvName),
                ("الوصف", govService.SrvDesc),
                ("الفئة", govService.Category?.Name ?? "غير محدد"),
                ("مدة التنفيذ", govService.SrvTime),
                ("الرسوم", govService.SrvFees.ToString())
            );

            chunks.Add(new ServiceChunkDTO
            {
                ChunkId = $"service_{govService.Id}_{ChunkType.Overview}",
                ServiceId = govService.Id,
                ChunkType = ChunkType.Overview.ToString(),
                Content = overview,
                Metadata = BuildMetadata(govService, ChunkType.Overview.ToString(),overview)
            });

            // =========================
            // Documents Chunk
            // =========================
            if (govService.RequiredDocuments.Any())
            {
                var documents = "المستندات المطلوبة:\n" +
                    string.Join("\n",
                        govService.RequiredDocuments.Select((d, i) =>
                            $"{i + 1}. {d.DocumentName}")
                    );

                chunks.Add(new ServiceChunkDTO
                {
                    ChunkId = $"service_{govService.Id}_{ChunkType.RequiredDocuments}",
                    ServiceId = govService.Id,
                    ChunkType = ChunkType.RequiredDocuments.ToString(),
                    Content =  documents,
                    Metadata = BuildMetadata(govService, ChunkType.RequiredDocuments.ToString(),documents)
                });
            }

            // =========================
            // Steps Chunk
            // =========================
            if (govService.ServiceSteps.Any())
            {
                var steps = "خطوات الخدمة:\n" +
                    string.Join("\n",
                        govService.ServiceSteps
                            .OrderBy(x => x.StepOrder)
                            .Select(x => $"{x.StepOrder}. {x.Title}")
                    );

                chunks.Add(new ServiceChunkDTO
                {
                    ChunkId = $"service_{govService.Id}_{ChunkType.Steps}",
                    ServiceId = govService.Id,
                    ChunkType = ChunkType.Steps.ToString(),
                    Content =  steps,
                    Metadata = BuildMetadata(govService, ChunkType.Steps.ToString(),steps)
                });
            }

            // =========================
            // Fees Chunk
            // =========================
            var fees = BuildBlock(
                ("الرسوم الأساسية", govService.SrvFees.ToString()),
                ("مدة التنفيذ", govService.SrvTime)
            );

            chunks.Add(new ServiceChunkDTO
            {
                ChunkId = $"service_{govService.Id}_{ChunkType.Fees}",
                ServiceId = govService.Id,
                ChunkType = ChunkType.Fees.ToString(),
                Content = fees,
                Metadata = BuildMetadata(govService, ChunkType.Fees.ToString(),fees)
            });

            return chunks;
        }
        private static Dictionary<string, object> BuildMetadata( GovService service,string chunkType,string content )
        {
            return new Dictionary<string, object>
            {
                ["ChunckId"] = $"{service.Id}_{chunkType}",
                ["ServiceId"] = service.Id,
                ["ServiceName"] = service.SrvName,
                ["CategoryId"] = service.CategoryId,
                ["CategoryName"] = service.Category?.Name ?? "",
                ["ChunckType"] = chunkType,
                ["Content"] = content,
                ["Language"] = "ar"
            };
        }
    }
}
