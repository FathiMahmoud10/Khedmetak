using Khedmetak.BLL.ApiResponse;
using Khedmetak.BLL.DTOS.Admin;
using Khedmetak.BLL.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;

// --Added by Naglaa
namespace Khedmetak.API.Controllers
{
    [ApiController]
    [Route("api/admin/govservices")]
    public class AdminStepsController : ControllerBase
    {
        private readonly IGovServiceAdminService _adminService;

        public AdminStepsController(IGovServiceAdminService adminService)
        {
            _adminService = adminService;
        }

       

        /*  عرض كل خطوات الخدمة مرتبة حسب StepOrder */
        [HttpGet("{id:int}/steps")]
        public async Task<IActionResult> GetSteps(int id)
        {
            var steps = await _adminService.GetStepsAsync(id);
            return Ok(ApiResponse<object>.Ok(steps));
        }

        /*  إضافة خطوة جديدة لمسار الخدمة */
        [HttpPost("{id:int}/steps")]
        public async Task<IActionResult> AddStep(int id, [FromBody] CreateServiceStepDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.Fail("Invalid input data."));

            var step = await _adminService.AddStepAsync(id, dto);
            if (step is null)
                return NotFound(ApiResponse<object>.Fail($"Service with id {id} not found."));

            return Ok(ApiResponse<object>.Ok(step, "Step added successfully."));
        }

        /*  تعديل خطوة (العنوان / الترتيب) */
        [HttpPut("{id:int}/steps/{stepId:int}")]
        public async Task<IActionResult> UpdateStep(int id, int stepId, [FromBody] UpdateServiceStepDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.Fail("Invalid input data."));

            var step = await _adminService.UpdateStepAsync(id, stepId, dto);
            if (step is null)
                return NotFound(ApiResponse<object>.Fail($"Step with id {stepId} not found for service {id}."));

            return Ok(ApiResponse<object>.Ok(step, "Step updated successfully."));
        }

        /*  حذف خطوة من مسار الخدمة */
        [HttpDelete("{id:int}/steps/{stepId:int}")]
        public async Task<IActionResult> DeleteStep(int id, int stepId)
        {
            var deleted = await _adminService.DeleteStepAsync(id, stepId);
            if (!deleted)
                return NotFound(ApiResponse<object>.Fail($"Step with id {stepId} not found for service {id}."));

            return Ok(ApiResponse<object>.Ok(null!, "Step deleted successfully."));
        }
    }
}
