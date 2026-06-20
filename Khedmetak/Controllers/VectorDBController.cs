using Khedmetak.AI.Services.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Khedmetak.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VectorDBController : ControllerBase
    {
       
            private readonly IVectorIndexingService _vectorIndexingService;

            public VectorDBController(
                IVectorIndexingService vectorIndexingService)
            {
                _vectorIndexingService = vectorIndexingService;
            }

            [HttpPost("index-service/{serviceId:int}")]
            public async Task<IActionResult> IndexService(int serviceId)
            {
                await _vectorIndexingService.IndexServiceAsync(serviceId);

                return Ok(new
                {
                    Message = $"Service {serviceId} indexed successfully."
                });
            }
    }
}


