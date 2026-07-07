using Khedmetak.AI.RAG;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shard.VectorDBInterfaces;

namespace Khedmetak.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class VectorDBController : ControllerBase
    {
       
            private readonly IVectorDBService _vectorIndexingService;
        private readonly IRagService _ragService;

        public VectorDBController(IVectorDBService vectorIndexingService,IRagService ragService)
            {
                _vectorIndexingService = vectorIndexingService;
                _ragService = ragService;
            }

        [HttpPost("Add-service/{serviceId:int}")]
        public async Task<IActionResult> IndexService(int serviceId)
        {
            await _vectorIndexingService.AddOrUpdateGovServiceToVectorDBAsync(serviceId);
            return Ok(new { Message = $"Service {serviceId} indexed successfully." });
        }

        [HttpPost("Delete-service/{serviceId:int}")]
        public async Task<IActionResult> DeleteService(int serviceId)
        {
            await _vectorIndexingService.DeleteGovServiceFromVectorDBAsync(serviceId);
            return Ok(new { Message = $"Service {serviceId} deleted successfully." });
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string question)
        {
            if (string.IsNullOrWhiteSpace(question))
                return BadRequest("Question is required.");

            var results =
                await _ragService.SearchServiceAsync(question);

            return Ok(results);
        }
    }
}
