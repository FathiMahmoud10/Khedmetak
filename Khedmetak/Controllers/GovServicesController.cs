using Khedmetak.BLL.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;

//  --Added By Fathi 

namespace Khedmetak.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GovServicesController : ControllerBase
    {
        private readonly IGovServiceService _govServiceService;

        public GovServicesController(IGovServiceService govServiceService)
        {
            _govServiceService = govServiceService;
        }
        /*  عرض كل الحكومات  */

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var services = await _govServiceService.GetAllServicesAsync();
            return Ok(services);
        }
        /*  عرض كل الحكومه لمحددة */

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var service = await _govServiceService.GetServiceDetailsAsync(id);
            if (service is null) return NotFound();
            return Ok(service);
        }
        /*  عرض كل الخدمات المرتبطه ب الحكومه المحددة */
        [HttpGet("by-category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var services = await _govServiceService.GetServicesByCategoryAsync(categoryId);
            return Ok(services);
        }
    }
}