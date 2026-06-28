using Khedmetak.AI.DTOs.RagDTOs;
using Khedmetak.AI.Services.Abstraction;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.shared;
using System;
using System.Threading.Tasks;

namespace Khedmetak.AI.Services.Implementation
{
    public class VectorDBService : IVectorDBService
    {
        private readonly IChunkService _chunkService;
        private readonly IEmbeddingService _embeddingService;
        private readonly IVectorDB _vectorDB;
        private readonly IGenericRepository<GovService> _serviceRepository;

        public VectorDBService(
            IChunkService chunkService,
            IEmbeddingService embeddingService,
            IVectorDB vectorDB,
            IGenericRepository<GovService> serviceRepository)
        {
            _chunkService = chunkService;
            _embeddingService = embeddingService;
            _vectorDB = vectorDB;
            _serviceRepository = serviceRepository;
        }

        public async Task AddOrUpdateGovServiceToVectorDBAsync(int serviceId)
        {
            var service = await _serviceRepository.GetByIdAsync(serviceId);
            if (service == null)
                throw new Exception($"Service {serviceId} not found");

            // Generate ONE overview chunk for the service
            var chunk = await _chunkService.GenerateServiceChunkAsync(serviceId);

            // Upsert it to the vector database
            await _vectorDB.UpsertServiceChunkAsync(chunk, _embeddingService.GenerateEmbeddingAsync);
        }

        public async Task DeleteGovServiceFromVectorDBAsync(int serviceId)
        {
            await _vectorDB.DeleteServiceChunksAsync(serviceId);
        }

        public async Task<RagServiceInfo?> GetServiceInfoFromVectorDBAsync(string userQuestion)
        {
            var embedding = await _embeddingService.GenerateEmbeddingAsync(userQuestion);
            return await _vectorDB.SearchServiceAsync(embedding);
        }
    }
}
