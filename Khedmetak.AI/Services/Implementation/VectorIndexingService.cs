using Khedmetak.AI.Services.Abstraction;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.Services.Implementation
{
    public class VectorIndexingService : IVectorIndexingService
    {
        private readonly IChunkService _chunkService;
        private readonly IEmbeddingService _embeddingService;
        private readonly IQdrantService _qdrantService;
        private readonly IGenericRepository<GovService> _serviceRepository;

        public VectorIndexingService(
            IChunkService chunkService,
            IEmbeddingService embeddingService,
           IQdrantService qdrantService,
            IGenericRepository<GovService> serviceRepository)
        {
            _chunkService = chunkService;
            _embeddingService = embeddingService;
            _qdrantService = qdrantService;
            _serviceRepository = serviceRepository;
        }

        public async Task IndexServiceAsync(int serviceId)
        {
            var service = await _serviceRepository.GetByIdAsync(
                serviceId,
                s => s.Category,
                s => s.ServiceSteps,
                s => s.RequiredDocuments
            );

            if (service == null)
                throw new Exception($"Service {serviceId} not found");

            var chunks = await _chunkService.GenerateChunksAsync(serviceId);

            await _qdrantService.UpsertServiceChunksAsync(
                service,
                chunks,
                _embeddingService.GenerateEmbeddingAsync
            );
        }
    }
}
