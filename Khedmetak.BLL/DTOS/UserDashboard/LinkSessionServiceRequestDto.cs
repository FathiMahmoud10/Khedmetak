using System;

namespace Khedmetak.BLL.DTOS.UserDashboard
{
    /// <summary>
    /// Matches the frontend's linkSessionToService(sessionGuidId, govServiceId, status) body:
    /// { sessionGuidId, govServiceId, status }
    /// </summary>
    public class LinkSessionServiceRequestDto
    {
        public Guid SessionGuidId { get; set; }
        public int? GovServiceId { get; set; }

        // Optional: Pending | InProgress | Completed | Rejected (defaults to Pending if not sent / invalid)
        public string? Status { get; set; }
    }
}
