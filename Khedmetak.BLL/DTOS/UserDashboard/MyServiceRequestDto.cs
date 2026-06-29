using System;

namespace Khedmetak.BLL.DTOS.UserDashboard
{
    /// <summary>
    /// Mirrors the frontend's MyServiceRequest interface exactly.
    /// </summary>
    public class MyServiceRequestDto
    {
        public int ChatSessionId { get; set; }
        public Guid SessionGuidId { get; set; }
        public int? GovServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;

        // Pending | InProgress | Completed | Rejected
        public string Status { get; set; } = string.Empty;

        // قيد الانتظار / قيد التنفيذ / مكتمل / مرفوض
        public string StatusLabel { get; set; } = string.Empty;

        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }

        public int MessagesCount { get; set; }
        public int UploadedDocumentsCount { get; set; }

        /// <summary>
        /// أول رسالة كتبها المستخدم في الشات — تُستخدم كـ preview للجلسة.
        /// </summary>
        public string Preview { get; set; } = string.Empty;
    }
}
