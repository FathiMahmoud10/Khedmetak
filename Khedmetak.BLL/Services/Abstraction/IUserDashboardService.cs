using Khedmetak.BLL.DTOS.UserDashboard;

namespace Khedmetak.BLL.Services.Abstraction
{
    public interface IUserDashboardService
    {
        Task<UserDashboardStatsDto> GetStatsAsync(int userId);

        Task<List<MyServiceRequestDto>> GetMyRequestsAsync(int userId);

        Task<bool> LinkSessionToServiceAsync(int userId, LinkSessionServiceRequestDto dto);
    }
}
