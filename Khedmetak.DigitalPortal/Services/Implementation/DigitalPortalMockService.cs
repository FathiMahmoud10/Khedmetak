using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Khedmetak.DigitalPortal.DTOs;
using Khedmetak.DigitalPortal.Services.Abstraction;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Entities.Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.Abstraction.UnitOfWork;
using Microsoft.Extensions.Configuration;
using Khedmetak.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace Khedmetak.DigitalPortal.Services.Implementation
{
    public class DigitalPortalMockService : IDigitalPortalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _context;
        private readonly string _uploadsRoot;

        // Mock government database of citizens
        private static readonly List<DigitalPortalCitizenDto> MockCitizens = new()
        {
            new()
            {
                NationalId = "29801011234567",
                PhoneNumber = "01012345678",
                FullName = "أحمد محمد علي السيد",
                DateOfBirth = new DateTime(1998, 1, 1),
                City = "القاهرة",
                District = "مصر الجديدة",
                Street = "شارع الثورة",
                BuildingNumber = "45",
                FloorNumber = "3",
                ApartmentNumber = "12",
                PostalCode = "11736"
            },
            new()
            {
                NationalId = "29505051234567",
                PhoneNumber = "01234567890",
                FullName = "منى أحمد محمود حسن",
                DateOfBirth = new DateTime(1995, 5, 5),
                City = "الجيزة",
                District = "الدقي",
                Street = "شارع التحرير",
                BuildingNumber = "120",
                FloorNumber = "5",
                ApartmentNumber = "20",
                PostalCode = "12311"
            },
            new()
            {
                NationalId = "30109091234567",
                PhoneNumber = "01122334455",
                FullName = "كريم يوسف مصطفى عثمان",
                DateOfBirth = new DateTime(2001, 9, 9),
                City = "الإسكندرية",
                District = "سموحة",
                Street = "شارع فيكتور عمانويل",
                BuildingNumber = "8",
                FloorNumber = "2",
                ApartmentNumber = "6",
                PostalCode = "21615"
            }
        };

        public DigitalPortalMockService(IUnitOfWork unitOfWork, AppDbContext context, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _uploadsRoot = configuration["UploadsPath"]
                ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        }

        public async Task<bool> SendOtpAsync(DigitalPortalLoginDto dto)
        {
            var citizenExists = MockCitizens.Any(c => 
                c.NationalId == dto.NationalId && 
                c.PhoneNumber == dto.PhoneNumber);

            if (!citizenExists)
            {
                citizenExists = await _context.CitizenProfiles.AnyAsync(c =>
                    c.NationalId == dto.NationalId && (c.User.PhoneNumber == dto.PhoneNumber || c.User.UserName == dto.PhoneNumber));
            }

            return citizenExists;
        }

        public async Task<DigitalPortalCitizenDto?> VerifyOtpAndGetCitizenAsync(DigitalPortalOtpDto dto)
        {
            if (dto.Otp != "123456")
            {
                return null;
            }

            var citizen = MockCitizens.FirstOrDefault(c => 
                c.NationalId == dto.NationalId && 
                c.PhoneNumber == dto.PhoneNumber);

            if (citizen == null)
            {
                var dbCitizen = await _context.CitizenProfiles
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.NationalId == dto.NationalId && (c.User.PhoneNumber == dto.PhoneNumber || c.User.UserName == dto.PhoneNumber));

                if (dbCitizen != null)
                {
                    citizen = new DigitalPortalCitizenDto
                    {
                        NationalId = dbCitizen.NationalId,
                        PhoneNumber = dbCitizen.User?.PhoneNumber ?? dto.PhoneNumber,
                        FullName = dbCitizen.FullName,
                        DateOfBirth = dbCitizen.DateOfBirth,
                        City = dbCitizen.City,
                        District = dbCitizen.District,
                        Street = dbCitizen.Street,
                        BuildingNumber = dbCitizen.BuildingNumber,
                        FloorNumber = dbCitizen.FloorNumber,
                        ApartmentNumber = dbCitizen.ApartmentNumber,
                        PostalCode = dbCitizen.PostalCode
                    };
                }
            }

            return citizen;
        }

        public async Task<SyncDocumentsResultDto> SyncCitizenDocumentsAsync(int userId, string nationalId)
        {
            var citizen = MockCitizens.FirstOrDefault(c => c.NationalId == nationalId);
            if (citizen == null)
            {
                var dbCitizen = await _context.CitizenProfiles
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.NationalId == nationalId);

                if (dbCitizen != null)
                {
                    citizen = new DigitalPortalCitizenDto
                    {
                        NationalId = dbCitizen.NationalId,
                        PhoneNumber = dbCitizen.User?.PhoneNumber ?? string.Empty,
                        FullName = dbCitizen.FullName,
                        DateOfBirth = dbCitizen.DateOfBirth,
                        City = dbCitizen.City,
                        District = dbCitizen.District,
                        Street = dbCitizen.Street,
                        BuildingNumber = dbCitizen.BuildingNumber,
                        FloorNumber = dbCitizen.FloorNumber,
                        ApartmentNumber = dbCitizen.ApartmentNumber,
                        PostalCode = dbCitizen.PostalCode
                    };
                }
            }

            if (citizen == null)
            {
                return new SyncDocumentsResultDto
                {
                    Success = false,
                    SyncedCount = 0,
                    Message = "المواطن غير مسجل في بوابة مصر الرقمية"
                };
            }

            var userFolder = Path.Combine(_uploadsRoot, userId.ToString());
            Directory.CreateDirectory(userFolder);

            var documentsToSync = new List<(string FileName, string Content, string FileType)>
            {
                (
                    "بطاقة_الرقم_القومي_الرقمية.pdf",
                    $"بوابة مصر الرقمية - وثيقة إلكترونية موثقة\n---------------------------------------\nالرقم القومي: {citizen.NationalId}\nالاسم الكامل: {citizen.FullName}\nتاريخ الميلاد: {citizen.DateOfBirth:dd/MM/yyyy}\nالعنوان: {citizen.BuildingNumber} {citizen.Street}، {citizen.District}، {citizen.City}\nتاريخ التوثيق: {DateTime.Now:dd/MM/yyyy HH:mm:ss}\nنسخة رقمية مؤمنة معتمدة للتقديم الحكومي.",
                    "application/pdf"
                ),
                (
                    "رخصة_القيادة_الرقمية.pdf",
                    $"بوابة مصر الرقمية - وثيقة إلكترونية موثقة\n---------------------------------------\nوزارة الداخلية المصرية - قطاع المرور\nرخصة قيادة خاصة\nالاسم: {citizen.FullName}\nالرقم القومي: {citizen.NationalId}\nفئة الرخصة: خاصة\nحالة الرخصة: سارية\nتاريخ التوثيق: {DateTime.Now:dd/MM/yyyy HH:mm:ss}",
                    "application/pdf"
                ),
                (
                    "شهادة_الميلاد_الرقمية.pdf",
                    $"وزارة الداخلية المصرية - مصلحة الأحوال المدنية\nشهادة ميلاد رقمية موثقة\n---------------------------------------\nالاسم الكامل: {citizen.FullName}\nالرقم القومي: {citizen.NationalId}\nتاريخ الميلاد: {citizen.DateOfBirth:dd/MM/yyyy}\nمحل الميلاد: {citizen.City}\nتاريخ التوثيق: {DateTime.Now:dd/MM/yyyy HH:mm:ss}",
                    "application/pdf"
                )
            };

            int syncedCount = 0;
            var existingDocs = await _unitOfWork.UserDocuments.GetByUserIdAsync(userId);

            foreach (var docInfo in documentsToSync)
            {
                if (existingDocs.Any(d => d.FileName == docInfo.FileName))
                {
                    continue;
                }

                var uniqueName = $"{Guid.NewGuid()}_{docInfo.FileName}";
                var fullPath = Path.Combine(userFolder, uniqueName);
                var relativePath = Path.Combine("uploads", userId.ToString(), uniqueName);

                await File.WriteAllTextAsync(fullPath, docInfo.Content);

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
                    ? $"تم سحب عدد {syncedCount} مستندات رسمية بنجاح من بوابة مصر الرقمية"
                    : "جميع المستندات مسحوبة ومحدثة بالفعل"
            };
        }

        public async Task<PortalSubmissionResultDto> SubmitAndIssueServiceRequestAsync(
            int userId,
            PortalSubmissionRequestDto dto)
        {
            var citizen = MockCitizens.FirstOrDefault(c => c.NationalId == dto.NationalId);
            if (citizen == null)
            {
                var dbCitizen = await _context.CitizenProfiles
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.NationalId == dto.NationalId);

                if (dbCitizen != null)
                {
                    citizen = new DigitalPortalCitizenDto
                    {
                        NationalId = dbCitizen.NationalId,
                        PhoneNumber = dbCitizen.User?.PhoneNumber ?? string.Empty,
                        FullName = dbCitizen.FullName,
                        DateOfBirth = dbCitizen.DateOfBirth,
                        City = dbCitizen.City,
                        District = dbCitizen.District,
                        Street = dbCitizen.Street,
                        BuildingNumber = dbCitizen.BuildingNumber,
                        FloorNumber = dbCitizen.FloorNumber,
                        ApartmentNumber = dbCitizen.ApartmentNumber,
                        PostalCode = dbCitizen.PostalCode
                    };
                }
            }

            if (citizen == null)
            {
                return new PortalSubmissionResultDto
                {
                    Success = false,
                    Message = "المواطن غير مسجل في بوابة مصر الرقمية"
                };
            }

            var docContent = $"بوابة مصر الرقمية - وثيقة إلكترونية موثقة وصادرة رسمياً\n" +
                             $"----------------------------------------------------\n" +
                             $"الرقم القومي: {citizen.NationalId}\n" +
                             $"الاسم الكامل: {citizen.FullName}\n" +
                             $"الخدمة: {dto.ServiceName}\n" +
                             $"تاريخ الإصدار: {DateTime.Now:dd/MM/yyyy HH:mm:ss}\n" +
                             $"رقم المعاملة: {Guid.NewGuid()}\n" +
                             $"حالة المستند: موثق ومعتمد رسمياً من الجهات الحكومية المختصة.";

            var docFileName = $"مستند_رسمي_مستخرج_{dto.ServiceName.Replace(" ", "_")}.pdf";

            var userFolder = Path.Combine(_uploadsRoot, userId.ToString());
            Directory.CreateDirectory(userFolder);

            var uniqueName = $"{Guid.NewGuid()}_{docFileName}";
            var fullPath = Path.Combine(userFolder, uniqueName);
            var relativePath = Path.Combine("uploads", userId.ToString(), uniqueName).Replace("\\", "/");

            await File.WriteAllTextAsync(fullPath, docContent);

            var entity = new UserDocument
            {
                UserId = userId,
                FileName = docFileName,
                FilePath = relativePath,
                FileType = "application/pdf",
                UploadedAt = DateTime.UtcNow,
                ValidationStatus = "Issued",
                ChatSessionId = null,
                RequiredDocumentId = null
            };

            _unitOfWork.UserDocuments.Add(entity);
            await _unitOfWork.SaveChangesAsync();

            var issuedDoc = new IssuedDocumentDto
            {
                FileName = docFileName,
                FileType = "application/pdf",
                FileBase64 = Convert.ToBase64String(
                    System.Text.Encoding.UTF8.GetBytes(docContent))
            };

            return new PortalSubmissionResultDto
            {
                Success = true,
                Message = $"تم إصدار المستند الرسمي بنجاح — «{docFileName}»",
                TransactionId = Guid.NewGuid().ToString(),
                IssuedDocument = issuedDoc
            };
        }
    }
}
