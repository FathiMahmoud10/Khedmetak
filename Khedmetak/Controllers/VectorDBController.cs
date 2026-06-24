using Khedmetak.AI.RAG;
using Khedmetak.AI.Services.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qdrant.Client.Grpc;

namespace Khedmetak.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VectorDBController : ControllerBase
    {
       
            private readonly IVectorDBOperationsService _vectorIndexingService;
        private readonly IRagService _ragService;

        public VectorDBController(
                IVectorDBOperationsService vectorIndexingService,IRagService ragService)
            {
                _vectorIndexingService = vectorIndexingService;
                _ragService = ragService;
            }

            [HttpPost("Add-service/{serviceId:int}")]
            public async Task<IActionResult> IndexService(int serviceId)
            {
                await _vectorIndexingService.AddGovServiceToVectorDBAsync(serviceId);

                return Ok(new
                {
                    Message = $"Service {serviceId} indexed successfully."
                });
            }

            [HttpPost("Delete-service/{serviceId:int}")]
            public async Task<IActionResult> DeleteService(int serviceId)
            {
                await _vectorIndexingService.DeleteGovServiceFromVectorDBAsync(serviceId);

                return Ok(new
                {
                    Message = $"Service {serviceId} deleted successfully."
                });
            }


        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string question)
        {
            if (string.IsNullOrWhiteSpace(question))
                return BadRequest("Question is required.");

            string results =
                await _ragService.RagPipeline(question);

            return Ok(results);
        }
    }
}


