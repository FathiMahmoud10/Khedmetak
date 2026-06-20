using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Khedmetak.AI.Configuration;
using Khedmetak.AI.DTOs;
using Khedmetak.AI.Services.Abstraction;
using Khedmetak.DAL.Entities;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Embeddings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.Services.Implementation
{
    public class EmbeddingService :IEmbeddingService
    {
        private readonly EmbeddingClient _embeddingClient;

        public EmbeddingService(OpenAIClient openAIClient, IOptions<AISettings> settings)
        {
            _embeddingClient = openAIClient.GetEmbeddingClient(settings.Value.EmbeddingModel);

        }

        public async Task<float[]> GenerateEmbeddingAsync(string text)
        {
            EmbeddingGenerationOptions options = new()
            {
                Dimensions = 768 // 1536 full size
            };
            OpenAIEmbedding embedding = await _embeddingClient.GenerateEmbeddingAsync(text,options);

            return embedding.ToFloats().ToArray();
        }

    
    }
}
