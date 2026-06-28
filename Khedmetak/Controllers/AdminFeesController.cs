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
    public class AdminFeesController : ControllerBase
    {
        private readonly IGovServiceAdminService _adminService;
        public AdminFeesController(IGovServiceAdminService adminService) => _adminService = adminService;

        [HttpPut("{id:int}/fees")]
        public async Task<IActionResult> UpdateFees(int id, [FromBody] UpdateFeesDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ApiResponse<object>.Fail("Invalid input data."));
            var updated = await _adminService.UpdateFeesAsync(id, dto);
            if (updated is null) return NotFound(ApiResponse<object>.Fail($"Service with id {id} not found."));
            return Ok(ApiResponse<object>.Ok(updated, "Fees updated successfully."));
        }
    }
}
