using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                return BadRequest(new { success = false, message = "المواطن غير مسجل في قاعدة بيانات بوابة مصر الرقمية" });

            return Ok(new { success = true, message = "تم إرسال كود التحقق 123456 بنجاح" });
        }

        // ──────────────────────────────────────────────────────────────
        // 2. التحقق من OTP وإرجاع بيانات المواطن
        // POST /api/external/portal/verify-otp
        // ──────────────────────────────────────────────────────────────
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
                return NotFound(new { success = false, message = "المواطن غير مسجل" });

            return Ok(new { success = true, data = citizen });
        }

        // ──────────────────────────────────────────────────────────────
        // 3. جلب مستندات المواطن
        // GET /api/external/portal/citizen-documents/{nationalId}
        // ──────────────────────────────────────────────────────────────
        [HttpGet("citizen-documents/{nationalId}")]
        public IActionResult GetCitizenDocuments(string nationalId)
        {
            var citizen = Citizens.FirstOrDefault(c => c.NationalId == nationalId);
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

        // سجل معاملات البوابة الرقمية الصادرة (static → مشتركة طوال دورة حياة التطبيق)
        public static readonly List<PortalTransactionRecord> Transactions = new();

        // ──────────────────────────────────────────────────────────────
        // 4. إصدار مستند رسمي عند قبول الطلب من الأدمن
        // POST /api/external/portal/issue-document
        // ──────────────────────────────────────────────────────────────
        [HttpPost("issue-document")]
        public IActionResult IssueDocument([FromBody] PortalIssueDocumentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NationalId) ||
                string.IsNullOrWhiteSpace(request.ServiceName))
                return BadRequest(new { success = false, message = "الرقم القومي واسم الخدمة مطلوبان" });

            var citizen = Citizens.FirstOrDefault(c => c.NationalId == request.NationalId);
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
                document      = doc
            });
        }

        // ──────────────────────────────────────────────────────────────
        // 5. استرجاع سجل المعاملات للتأكد من وصول الطلبات للبوابة
        // GET /api/external/portal/transactions
        // ──────────────────────────────────────────────────────────────
        [HttpGet("transactions")]
        public IActionResult GetTransactions()
        {
            return Ok(new { success = true, data = Transactions.OrderByDescending(t => t.Timestamp).ToList() });
        }

        // ── helper ──────────────────────────────────────────────────
        private static object BuildDocument(string fileName, string content) => new
        {
            fileName   = fileName,
            fileType   = "application/pdf",
            fileBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(content))
        };
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
