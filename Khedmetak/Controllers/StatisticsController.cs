using Khedmetak.BLL.ApiResponse;
using Khedmetak.BLL.DTOS.Statistics;
using Khedmetak.BLL.Services.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Khedmetak.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;
        public StatisticsController(IStatisticsService statisticsService) => _statisticsService = statisticsService;

        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            var result = await _statisticsService.GetStatisticsAsync();
            return Ok(ApiResponse<StatisticsDto>.Ok(result));
        }
    }
}
