using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Khedmetak.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace Khedmetak.API.Controllers
{
    /// <summary>
    /// محاكاة سيرفر بوابة مصر الرقمية — مدمجة داخل التطبيق الرئيسي.
    /// هذا الـ Controller يُمثّل دور EgyptDigitalPortal.SimulatedServer لحين
    /// الاتصال بالبوابة الحقيقية، بحيث لا يحتاج النشر لأي سيرفر خارجي إضافي.
    /// المسار الأساسي: /api/external/portal/*
    /// </summary>
    [ApiController]
    [Route("api/external/portal")]
    [AllowAnonymous]
    public class PortalSimulatedController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PortalSimulatedController(AppDbContext context)
        {
            _context = context;
        }
        // قاعدة بيانات المواطنين التجريبية (static → مشتركة طوال دورة حياة التطبيق)
        public static readonly List<MockCitizenRecord> Citizens = new()
        {
            new()
            {
                NationalId     = "29801011234567",
                PhoneNumber    = "01012345678",
                FullName       = "أحمد محمد علي السيد",
                DateOfBirth    = new DateTime(1998, 1, 1),
                City           = "القاهرة",
                District       = "مصر الجديدة",
                Street         = "شارع الثورة",
                BuildingNumber = "45",
                FloorNumber    = "3",
                ApartmentNumber= "12",
                PostalCode     = "11736"
            },
            new()
            {
                NationalId     = "29505051234567",
                PhoneNumber    = "01234567890",
                FullName       = "منى أحمد محمود حسن",
                DateOfBirth    = new DateTime(1995, 5, 5),
                City           = "الجيزة",
                District       = "الدقي",
                Street         = "شارع التحرير",
                BuildingNumber = "120",
                FloorNumber    = "5",
                ApartmentNumber= "20",
                PostalCode     = "12311"
            },
            new()
            {
                NationalId     = "30109091234567",
                PhoneNumber    = "01122334455",
                FullName       = "كريم يوسف مصطفى عثمان",
                DateOfBirth    = new DateTime(2001, 9, 9),
                City           = "الإسكندرية",
                District       = "سموحة",
                Street         = "شارع فيكتور عمانويل",
                BuildingNumber = "8",
                FloorNumber    = "2",
                ApartmentNumber= "6",
                PostalCode     = "21615"
            }
        };

        // ──────────────────────────────────────────────────────────────
        // 1. إرسال كود OTP
        // POST /api/external/portal/send-otp
        // ──────────────────────────────────────────────────────────────
        [HttpPost("send-otp")]
        public IActionResult SendOtp([FromBody] PortalSendOtpRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NationalId) ||
                string.IsNullOrWhiteSpace(request.PhoneNumber))
                return BadRequest(new { success = false, message = "الرقم القومي ورقم الهاتف مطلوبان" });

            var exists = Citizens.Any(c =>
                c.NationalId == request.NationalId && c.PhoneNumber == request.PhoneNumber);

            if (!exists)
            {
                exists = _context.CitizenProfiles.Any(c =>
                    c.NationalId == request.NationalId && (c.User.PhoneNumber == request.PhoneNumber || c.User.UserName == request.PhoneNumber));
            }

            if (!exists)
                return BadRequest(new { success = false, message = "المواطن غير مسجل في قاعدة بيانات بوابة مصر الرقمية" });

            return Ok(new { success = true, message = "تم إرسال كود التحقق 123456 بنجاح" });
        }

        // ميثود التحقق من كود OTP (التحقق من صحة الكود المرسل)
        [HttpPost("verify-otp")]
        public IActionResult VerifyOtp([FromBody] PortalVerifyOtpRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NationalId) ||
                string.IsNullOrWhiteSpace(request.Otp))
                return BadRequest(new { success = false, message = "البيانات المطلوبة ناقصة" });

            if (request.Otp != "123456")
                return BadRequest(new { success = false, message = "كود التحقق غير صحيح" });

            var citizen = Citizens.FirstOrDefault(c =>
                c.NationalId == request.NationalId && c.PhoneNumber == request.PhoneNumber);

            if (citizen == null)
            {
                var dbCitizen = _context.CitizenProfiles
                    .Include(c => c.User)
                    .FirstOrDefault(c => c.NationalId == request.NationalId && (c.User.PhoneNumber == request.PhoneNumber || c.User.UserName == request.PhoneNumber));

                if (dbCitizen != null)
                {
                    citizen = new MockCitizenRecord
                    {
                        NationalId = dbCitizen.NationalId,
                        PhoneNumber = dbCitizen.User?.PhoneNumber ?? request.PhoneNumber,
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
                return NotFound(new { success = false, message = "المواطن غير مسجل" });

            return Ok(new { success = true, data = citizen });
        }
        // مثود تجريبي لاسترجاع المستندات الرسمية للمواطن من بوابة مصر الرقمية
        [HttpGet("citizen-documents/{nationalId}")]
        public IActionResult GetCitizenDocuments(string nationalId)
        {
            var citizen = Citizens.FirstOrDefault(c => c.NationalId == nationalId);
            if (citizen == null)
            {
                var dbCitizen = _context.CitizenProfiles
                    .Include(c => c.User)
                    .FirstOrDefault(c => c.NationalId == nationalId);

                if (dbCitizen != null)
                {
                    citizen = new MockCitizenRecord
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
                return NotFound(new { success = false, message = "المواطن غير مسجل" });

            var documents = new[]
            {
                BuildDocument(
                    "بطاقة_الرقم_القومي_الرقمية.pdf",
                    $"بوابة مصر الرقمية - وثيقة إلكترونية موثقة\n" +
                    $"الرقم القومي: {citizen.NationalId}\nالاسم: {citizen.FullName}\n" +
                    $"تاريخ الميلاد: {citizen.DateOfBirth:dd/MM/yyyy}\n" +
                    $"العنوان: {citizen.BuildingNumber} {citizen.Street}، {citizen.District}، {citizen.City}\n" +
                    $"تاريخ التوثيق: {DateTime.Now:dd/MM/yyyy HH:mm:ss}"),

                BuildDocument(
                    "رخصة_القيادة_الرقمية.pdf",
                    $"وزارة الداخلية المصرية - قطاع المرور\n" +
                    $"رخصة قيادة خاصة\nالاسم: {citizen.FullName}\n" +
                    $"الرقم القومي: {citizen.NationalId}\nحالة الرخصة: سارية\n" +
                    $"تاريخ التوثيق: {DateTime.Now:dd/MM/yyyy HH:mm:ss}"),

                BuildDocument(
                    "شهادة_الميلاد_الرقمية.pdf",
                    $"وزارة الداخلية - مصلحة الأحوال المدنية\n" +
                    $"شهادة ميلاد رقمية\nالاسم: {citizen.FullName}\n" +
                    $"تاريخ الميلاد: {citizen.DateOfBirth:dd/MM/yyyy}\n" +
                    $"محل الميلاد: {citizen.City}\n" +
                    $"تاريخ التوثيق: {DateTime.Now:dd/MM/yyyy HH:mm:ss}")
            };

            return Ok(new { success = true, data = documents });
        }

        public static readonly List<PortalTransactionRecord> Transactions = new();

        // ميثود لاصدار مستند رسمي موثق من بوابة مصر الرقمية (محاكاة)
        [HttpPost("issue-document")]
        public IActionResult IssueDocument([FromBody] PortalIssueDocumentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NationalId) ||
                string.IsNullOrWhiteSpace(request.ServiceName))
                return BadRequest(new { success = false, message = "الرقم القومي واسم الخدمة مطلوبان" });

            var citizen = Citizens.FirstOrDefault(c => c.NationalId == request.NationalId);
            if (citizen == null)
            {
                var dbCitizen = _context.CitizenProfiles
                    .Include(c => c.User)
                    .FirstOrDefault(c => c.NationalId == request.NationalId);

                if (dbCitizen != null)
                {
                    citizen = new MockCitizenRecord
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
                Transactions.Add(new PortalTransactionRecord
                {
                    TransactionId = "—",
                    NationalId = request.NationalId,
                    CitizenName = "غير مسجل",
                    ServiceName = request.ServiceName,
                    Timestamp = DateTime.UtcNow,
                    Status = "Failed",
                    ErrorMessage = "المواطن غير مسجل في قاعدة بيانات بوابة مصر الرقمية"
                });

                return BadRequest(new
                {
                    success = false,
                    message = "المواطن غير مسجل في قاعدة بيانات بوابة مصر الرقمية"
                });
            }

            var transactionId = Guid.NewGuid().ToString();

            var content =
                $"بوابة مصر الرقمية - وثيقة إلكترونية موثقة وصادرة رسمياً\n" +
                $"----------------------------------------------------\n" +
                $"الرقم القومي : {citizen.NationalId}\n" +
                $"الاسم الكامل: {citizen.FullName}\n" +
                $"الخدمة       : {request.ServiceName}\n" +
                $"تاريخ الإصدار: {DateTime.Now:dd/MM/yyyy HH:mm:ss}\n" +
                $"رقم المعاملة : {transactionId}\n" +
                $"الحالة       : موثق ومعتمد رسمياً من الجهات الحكومية المختصة.";

            var doc = BuildDocument(
                $"مستند_رسمي_{request.ServiceName.Replace(" ", "_")}.pdf",
                content);

            // إضافة المعاملة إلى سجل البوابة لتأكيد وصولها
            Transactions.Add(new PortalTransactionRecord
            {
                TransactionId = transactionId,
                NationalId = citizen.NationalId,
                CitizenName = citizen.FullName,
                ServiceName = request.ServiceName,
                Timestamp = DateTime.UtcNow,
                Status = "Issued"
            });

            return Ok(new
            {
                success       = true,
                transactionId = transactionId,
                data          = doc,
                message       = $"تم إصدار مستند «{request.ServiceName}» بنجاح عبر بوابة مصر الرقمية"
            });
        }

        // ميثود لاسترجاع سجل المعاملات الصادرة من بوابة مصر الرقمية (محاكاة)
        [HttpGet("transactions")]
        public IActionResult GetTransactions()
        {
            return Ok(new { success = true, data = Transactions.OrderByDescending(t => t.Timestamp).ToList() });
        }

        // helper
        private static object BuildDocument(string fileName, string content)
        {
            var pdfBytes = BuildPdfBytes(content);
            return new
            {
                fileName   = fileName,
                fileType   = "application/pdf",
                fileBase64 = Convert.ToBase64String(pdfBytes)
            };
        }

        private static byte[] BuildPdfBytes(string content)
        {
            var lines = content.Split('\n');
            var streamBuilder = new StringBuilder();
            streamBuilder.Append("BT\n/F1 12 Tf\n18 TL\n50 750 Td\n");
            foreach (var line in lines)
            {
                var escapedLine = line.Replace("(", "\\(").Replace(")", "\\)").Trim();
                streamBuilder.Append($"({escapedLine}) Tj T*\n");
            }
            streamBuilder.Append("ET");

            var streamContent = streamBuilder.ToString();
            var streamBytes = Encoding.UTF8.GetBytes(streamContent);

            var objects = new List<string>
            {
                "%PDF-1.4\n",
                "1 0 obj\n<< /Type /Catalog /Pages 2 0 R >>\nendobj\n",
                "2 0 obj\n<< /Type /Pages /Kids [3 0 R] /Count 1 >>\nendobj\n",
                "3 0 obj\n<< /Type /Page /Parent 2 0 R /Resources << /Font << /F1 4 0 R >> >> /MediaBox [0 0 612 792] /Contents 5 0 R >>\nendobj\n",
                "4 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold >>\nendobj\n"
            };

            // Calculate offsets
            var offsets = new List<long>();
            long currentOffset = 0;

            var headerBytes = Encoding.ASCII.GetBytes(objects[0]);
            currentOffset += headerBytes.Length;

            for (int i = 1; i <= 4; i++)
            {
                offsets.Add(currentOffset);
                currentOffset += Encoding.UTF8.GetByteCount(objects[i]);
            }

            var streamHeader = $"5 0 obj\n<< /Length {streamBytes.Length} >>\nstream\n";
            var streamFooter = "\nendstream\nendobj\n";

            offsets.Add(currentOffset);
            currentOffset += Encoding.UTF8.GetByteCount(streamHeader) + streamBytes.Length + Encoding.UTF8.GetByteCount(streamFooter);

            using (var ms = new MemoryStream())
            {
                ms.Write(headerBytes, 0, headerBytes.Length);
                for (int i = 1; i <= 4; i++)
                {
                    var bytes = Encoding.UTF8.GetBytes(objects[i]);
                    ms.Write(bytes, 0, bytes.Length);
                }

                var shBytes = Encoding.UTF8.GetBytes(streamHeader);
                ms.Write(shBytes, 0, shBytes.Length);
                ms.Write(streamBytes, 0, streamBytes.Length);
                var sfBytes = Encoding.UTF8.GetBytes(streamFooter);
                ms.Write(sfBytes, 0, sfBytes.Length);

                long xrefOffset = ms.Position;

                var xrefBuilder = new StringBuilder();
                xrefBuilder.Append("xref\n0 6\n0000000000 65535 f \n");
                foreach (var offset in offsets)
                {
                    xrefBuilder.Append($"{offset:D10} 00000 n \n");
                }
                xrefBuilder.Append($"trailer\n<< /Size 6 /Root 1 0 R >>\nstartxref\n{xrefOffset}\n%%EOF\n");

                var xrefBytes = Encoding.ASCII.GetBytes(xrefBuilder.ToString());
                ms.Write(xrefBytes, 0, xrefBytes.Length);

                return ms.ToArray();
            }
        }
    }

    // ── Request models (local to this controller) ─────────────────
    public class PortalSendOtpRequest
    {
        public string NationalId  { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class PortalVerifyOtpRequest
    {
        public string NationalId  { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Otp         { get; set; } = string.Empty;
    }

    public class PortalIssueDocumentRequest
    {
        public string NationalId  { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
    }

    public class MockCitizenRecord
    {
        public string   NationalId      { get; set; } = string.Empty;
        public string   PhoneNumber     { get; set; } = string.Empty;
        public string   FullName        { get; set; } = string.Empty;
        public DateTime DateOfBirth     { get; set; }
        public string   City            { get; set; } = string.Empty;
        public string   District        { get; set; } = string.Empty;
        public string   Street          { get; set; } = string.Empty;
        public string   BuildingNumber  { get; set; } = string.Empty;
        public string   FloorNumber     { get; set; } = string.Empty;
        public string   ApartmentNumber { get; set; } = string.Empty;
        public string   PostalCode      { get; set; } = string.Empty;
    }

    public class PortalTransactionRecord
    {
        public string TransactionId { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
        public string CitizenName { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
