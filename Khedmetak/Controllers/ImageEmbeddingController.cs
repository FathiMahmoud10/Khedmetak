using Khedmetak.AI.Services.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Khedmetak.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageEmbeddingController : ControllerBase
    {
        private readonly IClipImageEmbeddingService _embeddingGenerator;

        public ImageEmbeddingController(IClipImageEmbeddingService embeddingGenerator)
        {
            _embeddingGenerator = embeddingGenerator;
        }

        [HttpPost("image")]
        public async Task<IActionResult> GenerateImageEmbedding(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("Please upload an image.");
            }

            await using var stream = image.OpenReadStream();

            float[] embedding = await _embeddingGenerator.GenerateEmbeddingAsync(stream);

            return Ok(new
            {
                Success = true,
                Dimensions = embedding.Length,
                Embedding = embedding
            });
        }
    }
}
