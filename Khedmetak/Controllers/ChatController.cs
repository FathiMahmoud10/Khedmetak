using Khedmetak.AI.Agents.Abstraction;
using Khedmetak.AI.Agents.Implementaion;
using Khedmetak.AI.DTOs.ChatSessionDTO;
using Khedmetak.AI.DTOs.UserAIChatDataDto;
using Khedmetak.AI.Services.Abstraction;
using Khedmetak.BLL.ApiResponse;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.Abstraction;
using Khedmetak.DAL.Repo.Abstraction.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace Khedmetak.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatOrchestrator _chatOrchestrator;
        private readonly IChatSessionService _chatSessionService;
        private readonly IServiceIntentAgent _Intent;
        private readonly IChatSessionRepository _chatSessionRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _uploadsRoot;

        public ChatController(
            IChatOrchestrator chatOrchestrator,
            IChatSessionService chatSessionService,
            IServiceIntentAgent intent,
            IChatSessionRepository chatSessionRepo,
            IUnitOfWork unitOfWork,
            IConfiguration configuration)
        {
            _chatOrchestrator = chatOrchestrator;
            _chatSessionService = chatSessionService;
            _Intent = intent;
            _chatSessionRepo = chatSessionRepo;
            _unitOfWork = unitOfWork;
            _uploadsRoot = configuration["UploadsPath"]
                ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        }

        [HttpPost]
        public async Task<IActionResult> Chat(
            [FromBody] UserMessageDTO request)
        {
            var session =
                await _chatSessionService.GetSessionLast15Messages(
                    request.SessionGuidId);

            if (session == null)
            {
                return NotFound("Session not found.");
            }

            var answer =
                await _chatOrchestrator.AskAsync(
                    request.Message,
                    session);

            return Ok(answer);
        }

        [HttpPost("intent")]
        public async Task<IActionResult> ServiceIntent(
           [FromBody] string request)
        {
            var answer =
                await _Intent.DetectIntentAsync(request);

            return Ok(answer);
        }

        /// <summary>
        /// رفع ملف من صفحة الشات — يشتغل مع guest و logged-in users.
        /// الملف بيترفع عبر الـ SessionGuidId بدل الـ UserId.
        /// </summary>
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromForm] ChatUploadDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Fail("بيانات غير صحيحة"));

            // ── التحقق من وجود الجلسة ──
            var session = await _chatSessionRepo.GetBySessionGuidAsync(dto.SessionGuidId);
            if (session == null)
                return NotFound(ApiResponse<string>.Fail("الجلسة غير موجودة"));

            // ── التحقق من حجم الملف (10MB max) ──
            const long maxFileSize = 10 * 1024 * 1024;
            if (dto.File.Length > maxFileSize)
                return BadRequest(ApiResponse<string>.Fail("حجم الملف أكبر من 10 ميجابايت"));

            // ── حفظ الملف على السيرفر ──
            var sessionFolder = Path.Combine(_uploadsRoot, "chat", dto.SessionGuidId.ToString());
            Directory.CreateDirectory(sessionFolder);

            var ext = Path.GetExtension(dto.File.FileName).ToLowerInvariant();
            var uniqueName = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(sessionFolder, uniqueName);
            var relativePath = Path.Combine("uploads", "chat", dto.SessionGuidId.ToString(), uniqueName)
                                   .Replace("\\", "/");

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }

            // ── تسجيل الملف في قاعدة البيانات ──
            var entity = new UserDocument
            {
                UserId = session.UserId ?? 0,
                ChatSessionId = session.Id,
                FileName = dto.File.FileName,
                FilePath = relativePath,
                FileType = dto.File.ContentType,
                UploadedAt = DateTime.UtcNow,
                ValidationStatus = "Pending",
                RequiredDocumentId = dto.RequiredDocumentId
            };

            _unitOfWork.UserDocuments.Add(entity);
            await _unitOfWork.SaveChangesAsync();

            // ── بناء الـ URL الكامل للملف ──
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var fileUrl = $"{baseUrl}/{relativePath}";

            var result = new ChatUploadResultDto
            {
                Id = entity.Id,
                FileName = entity.FileName,
                FilePath = entity.FilePath,
                FileUrl = fileUrl,
                FileType = entity.FileType,
                UploadedAt = entity.UploadedAt,
                RequiredDocumentId = entity.RequiredDocumentId
            };

            return Ok(ApiResponse<ChatUploadResultDto>.Ok(result, "تم رفع الملف بنجاح"));
        }
    }
}