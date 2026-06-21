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
    public class VectorDBOperationsService : IVectorDBOperationsService
    {
        private readonly IChunkService _chunkService;
        private readonly IEmbeddingService _embeddingService;
        private readonly IQdrantService _qdrantService;
        private readonly IGenericRepository<GovService> _serviceRepository;

        public VectorDBOperationsService(
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


        //     ================ Add New Service to Vector DataBase ==================
        public async Task AddGovServiceToVectorDBAsync(int serviceId)
        {
            var service = await _serviceRepository.GetByIdAsync(serviceId);


            if (service == null)
                throw new Exception($"Service {serviceId} not found");


            // 1- Split Service into chunks
            var chunks = await _chunkService.GenerateChunksAsync(serviceId);

            // 2- Add Service Chunks to vector database
            await _qdrantService.UpsertServiceChunksAsync( 
                chunks,
                _embeddingService.GenerateEmbeddingAsync
            );
        }


        //     =================== Delete Service From Vector Database =================
        public async Task DeleteGovServiceFromVectorDBAsync(int serviceId)
        {
            await _qdrantService.DeleteServiceChunksAsync(serviceId);
           
        }


        // ==================== Update Service to Vector Database "Qdrant database" ===================
        //public async Task UpdateGovServiceInVectorDBAsync(int serviceId)
        //{
        //    var service = await _serviceRepository.GetByIdAsync(serviceId);


        //    if (service == null)
        //        throw new Exception($"Service {serviceId} not found");

        //    // 1- Delete Service from Vector Database

        //    await _qdrantService.DeleteServiceChunksAsync(serviceId);

        //    // 2- split service into chunks  again
        //    var chunks = await _chunkService.GenerateChunksAsync(serviceId);

        //    // 3- Add service chunks to Vector DB again
        //    await _qdrantService.UpsertServiceChunksAsync(
        //        chunks,
        //        _embeddingService.GenerateEmbeddingAsync
        //    );
        //}
    }
}
