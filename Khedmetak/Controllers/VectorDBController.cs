using Khedmetak.AI.Services.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Khedmetak.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VectorDBController : ControllerBase
    {
       
            private readonly IVectorDBOperationsService _vectorIndexingService;

            public VectorDBController(
                IVectorDBOperationsService vectorIndexingService)
            {
                _vectorIndexingService = vectorIndexingService;
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

       
    }
}


