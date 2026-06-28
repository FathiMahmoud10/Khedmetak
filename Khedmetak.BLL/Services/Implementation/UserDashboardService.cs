using Khedmetak.BLL.DTOS.UserDashboard;
using Khedmetak.BLL.Services.Abstraction;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Enums;
using Khedmetak.DAL.Repo.Abstraction;
using Khedmetak.DAL.Repo.Abstraction.UnitOfWork;

namespace Khedmetak.BLL.Services.Implementation
{
    public class UserDashboardService : IUserDashboardService
    {
        private readonly IChatSessionRepository _chatSessionRepo;
        private readonly IUnitOfWork _unitOfWork;

        // Arabic labels shown in the UI badges, keyed by the enum name sent in JSON.
        private static readonly Dictionary<RequestStatus, string> StatusLabels = new()
        {
            { RequestStatus.Pending, "قيد الانتظار" },
            { RequestStatus.InProgress, "قيد التنفيذ" },
            { RequestStatus.Completed, "مكتمل" },
            { RequestStatus.Rejected, "مرفوض" }
        };

        public UserDashboardService(IChatSessionRepository chatSessionRepo, IUnitOfWork unitOfWork)
        {
            _chatSessionRepo = chatSessionRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<UserDashboardStatsDto> GetStatsAsync(int userId)
        {
            var sessions = await _chatSessionRepo.GetByUserIdWithDetailsAsync(userId);

            // A "request" is a session the user actually linked to a government service.
            var requests = sessions.Where(s => s.GovServiceId != null).ToList();

            // -------------------------------------------------------------------------
            // FIX: TotalUploadedFiles must count ALL documents that belong to the user
            // (whether uploaded inside a chat session or directly from "My Files"),
            // not just the ones attached to a ChatSession. Summing s.UserDocuments
            // only counted chat-linked files and silently ignored standalone uploads.
            // -------------------------------------------------------------------------
            var userDocuments = await _unitOfWork.UserDocuments.GetByUserIdAsync(userId);
            var totalUploadedFiles = userDocuments.Count();

            var stats = new UserDashboardStatsDto
            {
                TotalRequests = requests.Count,
                PendingCount = requests.Count(s => s.Status == RequestStatus.Pending),
                InProgressCount = requests.Count(s => s.Status == RequestStatus.InProgress),
                CompletedCount = requests.Count(s => s.Status == RequestStatus.Completed),
                RejectedCount = requests.Count(s => s.Status == RequestStatus.Rejected),
                TotalUploadedFiles = totalUploadedFiles,
                TotalChatSessions = sessions.Count,
                RecentRequests = requests
                    .OrderByDescending(s => s.StartedAt)
                    .Take(5)
                    .Select(ToDto)
                    .ToList()
            };

            return stats;
        }

        public async Task<List<MyServiceRequestDto>> GetMyRequestsAsync(int userId)
        {
            var sessions = await _chatSessionRepo.GetByUserIdWithDetailsAsync(userId);

            return sessions
                .Where(s => s.GovServiceId != null)
                .OrderByDescending(s => s.StartedAt)
                .Select(ToDto)
                .ToList();
        }

        public async Task<bool> LinkSessionToServiceAsync(int userId, LinkSessionServiceRequestDto dto)
        {
            var session = await _chatSessionRepo.GetBySessionGuidAsync(dto.SessionGuidId);
            if (session == null)
                return false;

            // Only the owner can link/update the session. If it isn't claimed yet (anonymous chat), claim it.
            if (session.UserId != null && session.UserId != userId)
                return false;

            session.UserId = userId;

            if (dto.GovServiceId.HasValue)
                session.GovServiceId = dto.GovServiceId;

            session.Status = ParseStatus(dto.Status);

            _chatSessionRepo.Update(session);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private static RequestStatus ParseStatus(string? status)
        {
            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<RequestStatus>(status, ignoreCase: true, out var parsed))
            {
                return parsed;
            }

            return RequestStatus.Pending;
        }

        private static MyServiceRequestDto ToDto(ChatSession s) => new()
        {
            ChatSessionId = s.Id,
            SessionGuidId = s.SessionGuid,
            GovServiceId = s.GovServiceId,
            ServiceName = s.GovService?.SrvName ?? string.Empty,
            CategoryName = s.Category?.Name ?? s.GovService?.Category?.Name ?? string.Empty,
            Status = s.Status.ToString(),
            StatusLabel = StatusLabels.TryGetValue(s.Status, out var label) ? label : s.Status.ToString(),
            StartedAt = s.StartedAt,
            EndedAt = s.EndedAt,
            MessagesCount = s.ChatMessages?.Count ?? 0,
            UploadedDocumentsCount = s.UserDocuments?.Count ?? 0
        };
    }
}