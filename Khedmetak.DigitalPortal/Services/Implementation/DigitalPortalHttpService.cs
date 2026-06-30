using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Khedmetak.DigitalPortal.DTOs;
using Khedmetak.DigitalPortal.Services.Abstraction;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Entities.Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.Abstraction.UnitOfWork;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace Khedmetak.DigitalPortal.Services.Implementation
{
    public class DigitalPortalHttpService : IDigitalPortalService
    {
        private readonly HttpClient _httpClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _uploadsRoot;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DigitalPortalHttpService(
            HttpClient httpClient, 
            IUnitOfWork unitOfWork, 
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor = null)
        {
            _httpClient = httpClient;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _uploadsRoot = configuration["UploadsPath"]
                ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        }

        private string GetBaseUri()
        {
            var baseUri = _httpClient.BaseAddress?.ToString();
            if (string.IsNullOrEmpty(baseUri) || baseUri.Contains("localhost:5200") || baseUri.Contains("localhost:5000"))
            {
                var httpContext = _httpContextAccessor?.HttpContext;
                if (httpContext != null)
                {
                    var request = httpContext.Request;
                    baseUri = $"{request.Scheme}://{request.Host}/";
                }
            }
            if (string.IsNullOrEmpty(baseUri))
            {
                baseUri = "https://iticon.runasp.net/";
            }
            if (!baseUri.EndsWith("/")) baseUri += "/";
            return baseUri;
        }

        public async Task<bool> SendOtpAsync(DigitalPortalLoginDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(GetBaseUri() + "api/external/portal/send-otp", dto);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<DigitalPortalCitizenDto?> VerifyOtpAndGetCitizenAsync(DigitalPortalOtpDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(GetBaseUri() + "api/external/portal/verify-otp", dto);
                if (!response.IsSuccessStatusCode)
                    return null;

                var result = await response.Content.ReadFromJsonAsync<ApiResponseWrapper<DigitalPortalCitizenDto>>();
                return result?.Data;
            }
            catch
            {
                return null;
            }
        }

        public async Task<SyncDocumentsResultDto> SyncCitizenDocumentsAsync(int userId, string nationalId)
        {
            try
            {
                var response = await _httpClient.GetAsync(GetBaseUri() + $"api/external/portal/citizen-documents/{nationalId}");
                if (!response.IsSuccessStatusCode)
                {
                    return new SyncDocumentsResultDto
                    {
                        Success = false,
                        SyncedCount = 0,
                        Message = "فشل الاتصال بخادم بوابة مصر الرقمية أو الرقم القومي غير مسجل"
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponseWrapper<List<ExternalDocumentDto>>>();
                if (result == null || result.Data == null)
                {
                    return new SyncDocumentsResultDto
                    {
                        Success = false,
                        SyncedCount = 0,
                        Message = "لم يتم العثور على مستندات لهذا المواطن"
                    };
                }

                var userFolder = Path.Combine(_uploadsRoot, userId.ToString());
                Directory.CreateDirectory(userFolder);

                int syncedCount = 0;
                var existingDocs = await _unitOfWork.UserDocuments.GetByUserIdAsync(userId);

                foreach (var docInfo in result.Data)
                {
                    if (existingDocs.Any(d => d.FileName == docInfo.FileName))
                    {
                        continue;
                    }

                    var fileBytes = Convert.FromBase64String(docInfo.FileBase64);
                    var uniqueName = $"{Guid.NewGuid()}_{docInfo.FileName}";
                    var fullPath = Path.Combine(userFolder, uniqueName);
                    var relativePath = Path.Combine("uploads", userId.ToString(), uniqueName);

                    await File.WriteAllBytesAsync(fullPath, fileBytes);

                    var entity = new UserDocument
                    {
                        UserId = userId,
                        FileName = docInfo.FileName,
                        FilePath = relativePath,
                        FileType = docInfo.FileType,
                        UploadedAt = DateTime.UtcNow,
                        ValidationStatus = "Verified",
                        ChatSessionId = null,
                        RequiredDocumentId = null
                    };

                    _unitOfWork.UserDocuments.Add(entity);
                    syncedCount++;
                }

                if (syncedCount > 0)
                {
                    await _unitOfWork.SaveChangesAsync();
                }

                return new SyncDocumentsResultDto
                {
                    Success = true,
                    SyncedCount = syncedCount,
                    Message = syncedCount > 0 
                        ? $"تم سحب عدد {syncedCount} مستندات رسمية بنجاح عبر الشبكة من بوابة مصر الرقمية"
                        : "جميع المستندات مسحوبة ومحدثة بالفعل"
                };
            }
            catch (Exception ex)
            {
                return new SyncDocumentsResultDto
                {
                    Success = false,
                    SyncedCount = 0,
                    Message = $"حدث خطأ أثناء الاتصال بالشبكة: {ex.Message}"
                };
            }
        }
        public async Task<PortalSubmissionResultDto> SubmitAndIssueServiceRequestAsync(
            int userId,
            PortalSubmissionRequestDto dto)
        {
            try
            {
                // إرسال طلب الإصدار عبر HTTP إلى سيرفر بوابة مصر الرقمية
                var response = await _httpClient.PostAsJsonAsync(
                    GetBaseUri() + "api/external/portal/issue-document", dto);

                if (!response.IsSuccessStatusCode)
                {
                    return new PortalSubmissionResultDto
                    {
                        Success = false,
                        Message = "رفضت البوابة الرقمية الطلب — تأكد من تسجيل المواطن في قاعدة البيانات"
                    };
                }

                // استيعاب الاستجابة من البوابة
                var result = await response.Content
                    .ReadFromJsonAsync<ApiResponseWrapper<IssuedDocumentDto>>();

                if (result?.Data == null)
                {
                    return new PortalSubmissionResultDto
                    {
                        Success = false,
                        Message = "استجابة البوابة الرقمية فارغة أو غير مكتملة"
                    };
                }

                var issuedDoc = result.Data;

                // حفظ الملف الصادر في مجلد المستخدم على السيرفر
                var userFolder = Path.Combine(_uploadsRoot, userId.ToString());
                Directory.CreateDirectory(userFolder);

                var fileBytes = Convert.FromBase64String(issuedDoc.FileBase64);
                var uniqueName = $"{Guid.NewGuid()}_{issuedDoc.FileName}";
                var fullPath = Path.Combine(userFolder, uniqueName);
                var relativePath = Path.Combine(
                    "uploads", userId.ToString(), uniqueName).Replace("\\", "/");

                await File.WriteAllBytesAsync(fullPath, fileBytes);

                // تسجيل المستند الصادر في قاعدة البيانات
                var entity = new UserDocument
                {
                    UserId = userId,
                    FileName = issuedDoc.FileName,
                    FilePath = relativePath,
                    FileType = issuedDoc.FileType,
                    UploadedAt = DateTime.UtcNow,
                    ValidationStatus = "Issued",   // مصدر من البوابة رسمياً
                    ChatSessionId = null,
                    RequiredDocumentId = null
                };

                _unitOfWork.UserDocuments.Add(entity);
                await _unitOfWork.SaveChangesAsync();

                return new PortalSubmissionResultDto
                {
                    Success = true,
                    Message = $"تم إصدار المستند الرسمي بنجاح عبر بوابة مصر الرقمية — «{issuedDoc.FileName}»",
                    IssuedDocument = issuedDoc
                };
            }
            catch (Exception ex)
            {
                return new PortalSubmissionResultDto
                {
                    Success = false,
                    Message = $"حدث خطأ أثناء الاتصال ببوابة مصر الرقمية: {ex.Message}"
                };
            }
        }
    }

    public class ApiResponseWrapper<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class ExternalDocumentDto
    {
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public string FileBase64 { get; set; } = string.Empty;
    }
}
