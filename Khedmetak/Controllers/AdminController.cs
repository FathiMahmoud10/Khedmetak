using Khedmetak.BLL.ApiResponse;
using Khedmetak.Core.Data;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Enums;
using Khedmetak.DAL.Repo.Abstraction.UnitOfWork;
using Khedmetak.DigitalPortal.DTOs;
using Khedmetak.DigitalPortal.Services.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Khedmetak.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDigitalPortalService _portalService;

        public AdminController(
            AppDbContext context,
            IUnitOfWork unitOfWork,
            IDigitalPortalService portalService)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _portalService = portalService;
        }

        /// <summary>
        /// جلب جميع الطلبات المرتبطة بخدمات حكومية مع بيانات المواطن والمستندات.
        /// GET /api/Admin/Requests
        /// </summary>
        [HttpGet("Requests")]
        public async Task<IActionResult> GetAllRequests()
        {
            var sessions = await _context.ChatSessions
                .Where(s => s.GovServiceId != null && s.UserId != null)
                .Include(s => s.GovService)
                    .ThenInclude(g => g!.Category)
                .Include(s => s.User)
                    .ThenInclude(u => u!.CitizenProfile)
                .Include(s => s.UserDocuments)
                .OrderByDescending(s => s.StartedAt)
                .ToListAsync();

            var result = sessions.Select(s => new AdminRequestDto
            {
                Id              = s.Id,
                SessionGuid     = s.SessionGuid,
                UserId          = s.UserId ?? 0,
                CitizenName     = s.User?.CitizenProfile?.FullName
                                  ?? s.User?.Name
                                  ?? "مواطن",
                NationalId      = s.User?.CitizenProfile?.NationalId ?? string.Empty,
                Phone           = s.User?.PhoneNumber ?? string.Empty,
                ServiceName     = s.GovService?.SrvName ?? string.Empty,
                CategoryName    = s.GovService?.Category?.Name ?? string.Empty,
                Status          = s.Status.ToString(),
                StatusLabel     = GetStatusLabel(s.Status),
                StartedAt       = s.StartedAt,
                Documents       = s.UserDocuments
                    .Select(d => new AdminDocumentDto
                    {
                        Id          = d.Id,
                        FileName    = d.FileName,
                        FilePath    = d.FilePath,
                        FileType    = d.FileType,
                        UploadedAt  = d.UploadedAt,
                        Status      = d.ValidationStatus
                    }).ToList()
            }).ToList();

            return Ok(ApiResponse<List<AdminRequestDto>>.Ok(result));
        }

        /// <summary>
        /// تحديث حالة طلب المواطن.
        /// عند تحديد الحالة "Completed" يُرسل طلب الإصدار إلى بوابة مصر الرقمية تلقائياً.
        /// PUT /api/Admin/Requests/{id}/status
        /// </summary>
        [HttpPut("Requests/{id:int}/status")]
        public async Task<IActionResult> UpdateRequestStatus(
            int id,
            [FromBody] UpdateRequestStatusDto dto)
        {
            var session = await _context.ChatSessions
                .Include(s => s.GovService)
                .Include(s => s.User)
                    .ThenInclude(u => u!.CitizenProfile)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (session == null)
                return NotFound(ApiResponse<string>.Fail("الطلب غير موجود"));

            // تحليل الحالة الجديدة
            if (!Enum.TryParse<RequestStatus>(dto.Status, ignoreCase: true, out var newStatus))
                return BadRequest(ApiResponse<string>.Fail("حالة الطلب غير معروفة"));

            session.Status = newStatus;

            PortalSubmissionResultDto? issuanceResult = null;

            // إذا قبل الأدمن الطلب → إصدار المستند الرسمي عبر البوابة الرقمية
            if (newStatus == RequestStatus.Completed)
            {
                var nationalId = session.User?.CitizenProfile?.NationalId;
                var serviceName = session.GovService?.SrvName ?? "خدمة حكومية";

                if (string.IsNullOrWhiteSpace(nationalId))
                {
                    return BadRequest(ApiResponse<string>.Fail("لا يمكن إكمال الطلب وإصداره لأن المواطن ليس لديه رقم قومي مسجل"));
                }

                try
                {
                    issuanceResult = await _portalService.SubmitAndIssueServiceRequestAsync(
                        session.UserId!.Value,
                        new PortalSubmissionRequestDto
                        {
                            NationalId  = nationalId,
                            ServiceName = serviceName
                        });

                    if (issuanceResult == null || !issuanceResult.Success)
                    {
                        var errorReason = issuanceResult?.Message ?? "فشل غير معروف في بوابة مصر الرقمية";
                        return BadRequest(ApiResponse<string>.Fail($"فشل إصدار المستند من بوابة مصر الرقمية: {errorReason}"));
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ApiResponse<string>.Fail($"خطأ في الاتصال ببوابة مصر الرقمية: {ex.Message}"));
                }

                session.EndedAt = DateTime.UtcNow;
            }

            _context.ChatSessions.Update(session);
            await _context.SaveChangesAsync();

            var message = newStatus == RequestStatus.Completed && issuanceResult?.Success == true
                ? $"تم قبول الطلب وإصدار المستند الرسمي بنجاح — {issuanceResult.Message}"
                : newStatus == RequestStatus.Rejected
                    ? "تم رفض الطلب وإشعار المواطن"
                    : "تم تحديث حالة الطلب بنجاح";

            return Ok(ApiResponse<object>.Ok(new
            {
                RequestId       = id,
                NewStatus       = newStatus.ToString(),
                IssuanceResult  = issuanceResult
            }, message));
        }

        /// <summary>
        /// جلب جميع المدفوعات المستلمة للوحة تحكم الأدمن.
        /// GET /api/Admin/Payments
        /// </summary>
        [HttpGet("Payments")]
        public async Task<IActionResult> GetAllPayments()
        {
            var payments = await _context.Payments
                .Include(p => p.User)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var result = payments.Select(p => new AdminPaymentDto
            {
                Id = p.Id,
                MerchantRefNum = p.MerchantRefNum,
                FawryRefNumber = p.FawryRefNumber,
                PaymentUrl = p.PaymentUrl,
                PaymentMethod = p.PaymentMethod,
                Amount = p.Amount,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                PaidAt = p.PaidAt,
                UserId = p.UserId,
                UserEmail = p.User != null && p.User.Email != null ? p.User.Email : string.Empty,
                UserName = p.User != null && p.User.Name != null ? p.User.Name : string.Empty
            }).ToList();

            return Ok(ApiResponse<List<AdminPaymentDto>>.Ok(result));
        }

        // ─── helpers ───────────────────────────────────────────────
        private static string GetStatusLabel(RequestStatus s) => s switch
        {
            RequestStatus.Pending    => "قيد الانتظار",
            RequestStatus.InProgress => "قيد التنفيذ",
            RequestStatus.Completed  => "مكتمل",
            RequestStatus.Rejected   => "مرفوض",
            _                        => s.ToString()
        };
    }

    // ─── Local DTOs ────────────────────────────────────────────────
    public class AdminRequestDto
    {
        public int     Id           { get; set; }
        public Guid    SessionGuid  { get; set; }
        public int     UserId       { get; set; }
        public string  CitizenName  { get; set; } = string.Empty;
        public string  NationalId   { get; set; } = string.Empty;
        public string  Phone        { get; set; } = string.Empty;
        public string  ServiceName  { get; set; } = string.Empty;
        public string  CategoryName { get; set; } = string.Empty;
        public string  Status       { get; set; } = string.Empty;
        public string  StatusLabel  { get; set; } = string.Empty;
        public DateTime StartedAt  { get; set; }
        public List<AdminDocumentDto> Documents { get; set; } = new();
    }

    public class AdminDocumentDto
    {
        public int      Id         { get; set; }
        public string   FileName   { get; set; } = string.Empty;
        public string   FilePath   { get; set; } = string.Empty;
        public string   FileType   { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public string   Status     { get; set; } = string.Empty;
    }

    public class UpdateRequestStatusDto
    {
        /// <summary>Pending | InProgress | Completed | Rejected</summary>
        public string Status { get; set; } = string.Empty;
    }

    public class AdminPaymentDto
    {
        public int Id { get; set; }
        public string MerchantRefNum { get; set; } = string.Empty;
        public string? FawryRefNumber { get; set; }
        public string? PaymentUrl { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
    }
}
