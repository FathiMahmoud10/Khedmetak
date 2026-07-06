using Khedmetak.BLL.ApiResponse;
using Khedmetak.BLL.DTOS.Admin;
using Khedmetak.BLL.Services.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Khedmetak.API.Controllers
{
    [ApiController]
    [Route("api/admin/govservices")]
    [Authorize(Roles = "Admin")]
    public class AdminFeeTiersController : ControllerBase
    {
        private readonly IGovServiceAdminService _adminService;
        public AdminFeeTiersController(IGovServiceAdminService adminService) => _adminService = adminService;

        [HttpGet("{id:int}/fee-tiers")]
        public async Task<IActionResult> GetFeeTiers(int id)
        {
            var tiers = await _adminService.GetFeeTiersAsync(id);
            return Ok(ApiResponse<object>.Ok(tiers));
        }

        [HttpPost("{id:int}/fee-tiers")]
        public async Task<IActionResult> AddFeeTier(int id, [FromBody] CreateServiceFeeTierDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ApiResponse<object>.Fail("Invalid input data."));
            var tier = await _adminService.AddFeeTierAsync(id, dto);
            if (tier is null) return NotFound(ApiResponse<object>.Fail($"Service with id {id} not found."));
            return Ok(ApiResponse<object>.Ok(tier, "Fee tier added successfully."));
        }

        [HttpPut("{id:int}/fee-tiers/{tierId:int}")]
        public async Task<IActionResult> UpdateFeeTier(int id, int tierId, [FromBody] UpdateServiceFeeTierDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ApiResponse<object>.Fail("Invalid input data."));
            var tier = await _adminService.UpdateFeeTierAsync(id, tierId, dto);
            if (tier is null) return NotFound(ApiResponse<object>.Fail($"Fee tier with id {tierId} not found for service {id}."));
            return Ok(ApiResponse<object>.Ok(tier, "Fee tier updated successfully."));
        }

        [HttpDelete("{id:int}/fee-tiers/{tierId:int}")]
        public async Task<IActionResult> DeleteFeeTier(int id, int tierId)
        {
            var deleted = await _adminService.DeleteFeeTierAsync(id, tierId);
            if (!deleted) return NotFound(ApiResponse<object>.Fail($"Fee tier with id {tierId} not found for service {id}."));
            return Ok(ApiResponse<object>.Ok(null!, "Fee tier deleted successfully."));
        }
    }
}
