using Khedmetak.AI.DTOs;
using Khedmetak.AI.Services.Abstraction;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.shared;


namespace Khedmetak.AI.Services.Implementation
{
    public class ChunkService :IChunkService
    {
        private readonly IGenericRepository<GovService> serviceRepository;

        public ChunkService(IGenericRepository<GovService> repo)
        {
            serviceRepository = repo;
        }

        // ====================== Split Service To Chunks ======================
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
                (" اسم الخدمة", govService.SrvName),
                (" الوصف", govService.SrvDesc),
                (" الفئة", govService.Category?.Name ?? "غير محدد"),
                (" مدة التنفيذ", govService.SrvTime),
                (" الرسوم", govService.SrvFees.ToString())
            );

            chunks.Add(new ServiceChunkDTO
            {
                ChunkId = $"service_{govService.Id}_{ChunkType.Overview}",
                ServiceId = govService.Id,
                ChunkType = ChunkType.Overview.ToString(),
                Content = overview,
                CategoryId = govService.CategoryId,
                CategoryName = govService.Category.Name,
                ServiceName = govService.SrvName
                //Metadata = BuildMetadata(govService, ChunkType.Overview.ToString(),overview)
            });

            // =========================
            // Documents Chunk
            // =========================
            if (govService.RequiredDocuments.Any())
            {
                var documents = $"{govService.SrvName} المستندات المطلوبة ل :\n" +
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
                    CategoryId = govService.CategoryId,
                    CategoryName = govService.Category.Name,
                    ServiceName = govService.SrvName
                    //Metadata = BuildMetadata(govService, ChunkType.RequiredDocuments.ToString(),documents)
                });
            }

            // =========================
            // Steps Chunk
            // =========================
            if (govService.ServiceSteps.Any())
            {
                var steps = $" {govService.SrvName} خطوات التقديم على   :\n" +
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
                    CategoryId = govService.CategoryId,
                    CategoryName = govService.Category.Name,
                    ServiceName = govService.SrvName
                    //Metadata = BuildMetadata(govService, ChunkType.Steps.ToString(),steps)
                });
            }

            // =========================
            // Fees Chunk
            // =========================
            var fees = BuildBlock(
                ("اسم الخدمة ",govService.SrvName),
                ("الرسوم الأساسية", govService.SrvFees.ToString()),
                ("مدة التنفيذ", govService.SrvTime)
            );

            chunks.Add(new ServiceChunkDTO
            {
                ChunkId = $"service_{govService.Id}_{ChunkType.Fees}",
                ServiceId = govService.Id,
                ChunkType = ChunkType.Fees.ToString(),
                Content = fees,
                CategoryId = govService.CategoryId,
                CategoryName = govService.Category.Name,
                ServiceName = govService.SrvName
                //Metadata = BuildMetadata(govService, ChunkType.Fees.ToString(),fees)
            });

            return chunks;
        }


        //private string GetKeywordsForService(GovService service)
        //{
        //    return service.Id switch
        //    {
        //        1 => "بطاقة رقم قومي\nرقم قومي\nبطاقة شخصية\nNational ID\nاستخراج بطاقة",
        //        2 => "تجديد بطاقة\nبطاقة شخصية\nرقم قومي\nتجديد رقم قومي\nNational ID",
        //        3 => "رخصة سيارة\nرخصة مركبة\nمرور\nتجديد رخصة\nCar license",
        //        4 => "شهادة ميلاد\nبدل فاقد\nاستخراج شهادة ميلاد\nBirth certificate\nميلاد",
        //        _ => string.Join("\n", service.SrvName.Split(' ').Concat(service.SrvDesc.Split(' ')).Distinct().Where(w => w.Length > 3))
        //    };
        //}

        public async Task<ServiceChunkDTO> GenerateServiceChunkAsync(int serviceId)
        {
            var service = await serviceRepository.GetByIdAsync(
                serviceId,
                s => s.Category
            );

            if (service is null)
                throw new Exception($"Service {serviceId} not found.");

            //var keywords = GetKeywordsForService(service);

            var content = $"""
اسم الخدمة:
{service.SrvName}

الوصف:
{service.SrvDesc}

الفئة:
{service.Category?.Name ?? "غير محدد"}

""";

            return new ServiceChunkDTO
            {
                ChunkId = $"service_{service.Id}",
                ServiceId = service.Id,
                ChunkType = ChunkType.Overview.ToString(),

                Content = content,

                CategoryId = service.CategoryId,
                CategoryName = service.Category?.Name ?? "",
                ServiceName = service.SrvName
            };
        }

        //private static Dictionary<string, object> BuildMetadata( GovService service,string chunkType,string content )
        //{
        //    return new Dictionary<string, object>
        //    {
        //        ["ChunckId"] = $"{service.Id}_{chunkType}",
        //        ["ServiceId"] = service.Id,
        //        ["ServiceName"] = service.SrvName,
        //        ["CategoryId"] = service.CategoryId,
        //        ["CategoryName"] = service.Category?.Name ?? "",
        //        ["ChunckType"] = chunkType,
        //        ["Content"] = content,
        //        ["Language"] = "ar"
        //    };
        //}
    }
}
