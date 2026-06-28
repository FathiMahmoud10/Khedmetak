using Khedmetak.AI.DTOs.RagDTOs;
using Khedmetak.AI.Services.Abstraction;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.shared;
using Qdrant.Client.Grpc;
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
        private readonly IVectorDB vectorDB;
        private readonly IGenericRepository<GovService> _serviceRepository;

        public VectorDBOperationsService( IChunkService chunkService,IEmbeddingService embeddingService,
                                          IVectorDB qdrantVectorDB,IGenericRepository<GovService> serviceRepository)
        {
            _chunkService = chunkService;
            _embeddingService = embeddingService;
            vectorDB = qdrantVectorDB;
            _serviceRepository = serviceRepository;
        }


        //     ================ Add or upadte GovService 4 points to Vector DataBase ==================
        public async Task AddOrUpdateGovServiceToVectorDBAsync(int serviceId)
        {
            var service = await _serviceRepository.GetByIdAsync(serviceId);


            if (service == null)
                throw new Exception($"Service {serviceId} not found");

            // 1- Split Service into chunks
            var chunks = await _chunkService.GenerateChunksAsync(serviceId);

            // 2- Add Service Chunks to vector database
            await vectorDB.UpsertServiceChunksAsync(chunks, _embeddingService.GenerateEmbeddingAsync);
        }


        //     ================ Add or upadte GovService 1 point to Vector DataBase ==================
        public async Task AddOrUpdateGovServiceOveriewToVectorDBAsync(int serviceId)
        {
            var service = await _serviceRepository.GetByIdAsync(serviceId);


            if (service == null)
                throw new Exception($"Service {serviceId} not found");

            // 1- convert Service info into chunk
            var chunk = await _chunkService.GenerateServiceChunkAsync(serviceId);

            // 2- Add Service Chunks to vector database
            await vectorDB.UpsertServiceChunkAsync(chunk, _embeddingService.GenerateEmbeddingAsync);
        }

        //     =================== Delete Service From Vector Database =================
        public async Task DeleteGovServiceFromVectorDBAsync(int serviceId)
        {
            await vectorDB.DeleteServiceChunksAsync(serviceId);
           
        }


        //     =================== Search in Vector Database to return the service id of the most relative chunk to user question =================

        public async Task<RagServiceInfo?> GetServiceInfoFromVectorDBAsync(string userQustion)
        {
            // 1- Embedding user question
            var userQuestionEmbedding = await _embeddingService.GenerateEmbeddingAsync(userQustion);
            
            // 2- Search in vector DB using question embedding
            RagServiceInfo? result = await vectorDB.SearchServiceAsync(userQuestionEmbedding);
           
            return result;
        }
        //     =================== Search in Vector Database to return the most relative chunks to user question =================

        public async Task<IReadOnlyList<ScoredPoint>> SearchInVectorDBAsync(string userQustion)
        {
            // 1- Embedding user question
            var userQuestionEmbedding = await _embeddingService.GenerateEmbeddingAsync(userQustion);

            // 2- Search in vector DB using question embedding
            var results = await vectorDB.SearchInVectorDBAsync(userQuestionEmbedding);
            return results;
        }




    }
}
