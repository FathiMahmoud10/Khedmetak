using System.Collections.Generic;

namespace Khedmetak.BLL.DTOS.UserDashboard
{
    /// <summary>
    /// Mirrors the frontend's UserDashboardStats interface exactly.
    /// </summary>
    public class UserDashboardStatsDto
    {
        public int TotalRequests { get; set; }
        public int PendingCount { get; set; }
        public int InProgressCount { get; set; }
        public int CompletedCount { get; set; }
        public int RejectedCount { get; set; }
        public int TotalUploadedFiles { get; set; }
        public int TotalChatSessions { get; set; }
        public List<MyServiceRequestDto> RecentRequests { get; set; } = new List<MyServiceRequestDto>();
    }
}
