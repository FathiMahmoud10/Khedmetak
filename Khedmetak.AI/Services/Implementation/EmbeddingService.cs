using Khedmetak.AI.Configuration;
using Khedmetak.AI.DTOs.EmbeddingDTOs;
using Khedmetak.AI.Services.Abstraction;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;
using System.Net.Http;
namespace Khedmetak.AI.Services.Implementation
{
    public class EmbeddingService : IEmbeddingService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AISettings _settings;

        public EmbeddingService(
            IHttpClientFactory httpClientFactory,
            IOptions<AISettings> settings)
        {
            _httpClientFactory = httpClientFactory;
            _settings = settings.Value;
        }

        public async Task<float[]> GenerateEmbeddingAsync(string text)
        {
            var client = _httpClientFactory.CreateClient("jina");

            var request = new JinaEmbeddingRequest
            {
                Model = _settings.EmbeddingModel,
                Input = text,
                Dimensions = 768
            };

            var response = await client.PostAsJsonAsync(
                "/v1/embeddings",
                request);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<JinaEmbeddingResponse>();

            if (result == null ||
                result.Data == null ||
                result.Data.Count == 0)
            {
                throw new Exception("No embedding returned from Jina.");
            }

            return result.Data[0].Embedding.ToArray();
        }
    }
}