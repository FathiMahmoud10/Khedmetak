using Khedmetak.BLL.ApiResponse;
using Khedmetak.BLL.DTOS.UserDashboard;
using Khedmetak.BLL.Services.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Khedmetak.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserDashboardController : ControllerBase
    {
        private readonly IUserDashboardService _userDashboardService;

        public UserDashboardController(IUserDashboardService userDashboardService)
        {
            _userDashboardService = userDashboardService;
        }

        // GET api/UserDashboard/stats
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(ApiResponse<string>.Fail("غير مصرح لك، يرجى تسجيل الدخول أولاً"));

            var result = await _userDashboardService.GetStatsAsync(userId.Value);
            return Ok(ApiResponse<UserDashboardStatsDto>.Ok(result));
        }

        // GET api/UserDashboard/my-requests
        [HttpGet("my-requests")]
        public async Task<IActionResult> GetMyRequests()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(ApiResponse<string>.Fail("غير مصرح لك، يرجى تسجيل الدخول أولاً"));

            var result = await _userDashboardService.GetMyRequestsAsync(userId.Value);
            return Ok(ApiResponse<List<MyServiceRequestDto>>.Ok(result));
        }

        // POST api/UserDashboard/link-session-service
        [HttpPost("link-session-service")]
        public async Task<IActionResult> LinkSessionToService([FromBody] LinkSessionServiceRequestDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(ApiResponse<string>.Fail("غير مصرح لك، يرجى تسجيل الدخول أولاً"));

            var success = await _userDashboardService.LinkSessionToServiceAsync(userId.Value, dto);
            if (!success)
                return NotFound(ApiResponse<string>.Fail("الجلسة غير موجودة أو لا تخصك"));

            return Ok(ApiResponse<string>.Ok("تم الربط بنجاح", "تم تحديث الطلب بنجاح"));
        }

        private int? GetCurrentUserId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idClaim, out var id) ? id : null;
        }
    }
}
