using System;

namespace Khedmetak.DigitalPortal.DTOs
{
    public class DigitalPortalLoginDto
    {
        public string NationalId { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class DigitalPortalOtpDto
    {
        public string NationalId { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
    }

    public class DigitalPortalCitizenDto
    {
        public string FullName { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string BuildingNumber { get; set; } = string.Empty;
        public string FloorNumber { get; set; } = string.Empty;
        public string ApartmentNumber { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
    }

    public class SyncDocumentsResultDto
    {
        public bool Success { get; set; }
        public int SyncedCount { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// يُرسَل إلى البوابة الرقمية لإصدار مستند رسمي عند قبول طلب المواطن.
    /// </summary>
    public class PortalSubmissionRequestDto
    {
        public string NationalId { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
    }

    /// <summary>
    /// المستند الرسمي الصادر من البوابة الرقمية (Base64).
    /// </summary>
    public class IssuedDocumentDto
    {
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public string FileBase64 { get; set; } = string.Empty;
    }

    /// <summary>
    /// نتيجة عملية الإرسال والإصدار من البوابة الرقمية.
    /// </summary>
    public class PortalSubmissionResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? TransactionId { get; set; }
        public IssuedDocumentDto? IssuedDocument { get; set; }
    }
}
