//using System;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;
//using Khedmetak.AI.Services.Abstraction;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;

//namespace Khedmetak.Controllers
//{
//    [ApiController]
//    [Route("api/image-rag")]
//    public class UserImageRagController : ControllerBase
//    {
//        private readonly IImageVectorDbService _vectorDbService;
//        private readonly ILogger<UserImageRagController> _logger;
//        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp" };

//        public UserImageRagController(
//            IImageVectorDbService vectorDbService,
//            ILogger<UserImageRagController> logger)
//        {
//            _vectorDbService = vectorDbService;
//            _logger = logger;
//        }

//        /// <summary>
//        /// POST /api/image-rag/search
//        /// Search KhedmetakImagesCollection for the best match using the uploaded image.
//        /// </summary>
//        [HttpPost("search")]
//        [Consumes("multipart/form-data")]
//        public async Task<IActionResult> Search(IFormFile image)
//        {
//            _logger.LogInformation("User Search: Starting image search request.");

//            if (image == null || image.Length == 0)
//            {
//                return BadRequest(new { success = false, message = "Empty image: Please upload a valid image file." });
//            }

//            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
//            if (!AllowedExtensions.Contains(extension))
//            {
//                return BadRequest(new { success = false, message = "Invalid image: Only standard image formats (JPG, PNG, BMP, GIF, WEBP) are allowed." });
//            }

//            try
//            {
//                await using var stream = image.OpenReadStream();
//                var result = await _vectorDbService.SearchAsync(stream);

//                if (!result.Success)
//                {
//                    return Ok(new { success = false, message = result.Message });
//                }

//                return Ok(new
//                {
//                    success = true,
//                    fileName = result.FileName,
//                    similarityScore = result.SimilarityScore
//                });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Errors: User search failed due to a server error.");
//                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "Qdrant unavailable or search failure." });
//            }
//        }
//    }
//}
