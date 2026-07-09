using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Configure port directly to http://localhost:5200
builder.WebHost.UseUrls("http://localhost:5200");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseRouting();

// Mock citizen database
var mockCitizens = new List<MockCitizen>
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
    },
    new()
    {
        NationalId = "30202022303798",
        PhoneNumber = "01097421321",
        FullName = "فتحي محمود أحمد",
        DateOfBirth = new DateTime(2002, 2, 2),
        City = "القاهرة",
        District = "المعادي",
        Street = "شارع 9",
        BuildingNumber = "12",
        FloorNumber = "4",
        ApartmentNumber = "8",
        PostalCode = "11728"
    }
};

// Endpoints
app.MapPost("/api/external/portal/send-otp", (SendOtpRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.NationalId) || string.IsNullOrWhiteSpace(request.PhoneNumber))
    {
        return Results.BadRequest(new { success = false, message = "الرقم القومي ورقم الهاتف مطلوبان" });
    }

    var exists = mockCitizens.Any(c => c.NationalId == request.NationalId && c.PhoneNumber == request.PhoneNumber);
    if (!exists)
    {
        return Results.BadRequest(new { success = false, message = "المواطن غير مسجل في قاعدة بيانات بوابة مصر الرقمية" });
    }

    return Results.Ok(new { success = true, message = "تم إرسال كود التحقق 123456 بنجاح" });
});

app.MapPost("/api/external/portal/verify-otp", (VerifyOtpRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.NationalId) || string.IsNullOrWhiteSpace(request.Otp))
    {
        return Results.BadRequest(new { success = false, message = "البيانات المطلوبة ناقصة" });
    }

    if (request.Otp != "123456")
    {
        return Results.BadRequest(new { success = false, message = "كود التحقق غير صحيح" });
    }

    var citizen = mockCitizens.FirstOrDefault(c => c.NationalId == request.NationalId && c.PhoneNumber == request.PhoneNumber);
    if (citizen == null)
    {
        return Results.NotFound(new { success = false, message = "المواطن غير مسجل" });
    }

    return Results.Ok(new { success = true, data = citizen });
});

app.MapGet("/api/external/portal/citizen-documents/{nationalId}", (string nationalId) =>
{
    var citizen = mockCitizens.FirstOrDefault(c => c.NationalId == nationalId);
    if (citizen == null)
    {
        return Results.NotFound(new { success = false, message = "المواطن غير مسجل" });
    }

    // Generate documents with simulated content, encoded in Base64
    var doc1Content = $"بوابة مصر الرقمية - وثيقة إلكترونية موثقة\n---------------------------------------\nالرقم القومي: {citizen.NationalId}\nالاسم الكامل: {citizen.FullName}\nتاريخ الميلاد: {citizen.DateOfBirth:dd/MM/yyyy}\nالعنوان: {citizen.BuildingNumber} {citizen.Street}، {citizen.District}، {citizen.City}\nتاريخ التوثيق: {DateTime.Now:dd/MM/yyyy HH:mm:ss}\nنسخة رقمية مؤمنة معتمدة للتقديم الحكومي.";
    var doc2Content = $"بوابة مصر الرقمية - وثيقة إلكترونية موثقة\n---------------------------------------\nوزارة الداخلية المصرية - قطاع المرور\nرخصة قيادة خاصة\nالاسم: {citizen.FullName}\nالرقم القومي: {citizen.NationalId}\nفئة الرخصة: خاصة\nحالة الرخصة: سارية\nتاريخ التوثيق: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
    var doc3Content = $"وزارة الداخلية المصرية - مصلحة الأحوال المدنية\nشهادة ميلاد رقمية موثقة\n---------------------------------------\nالاسم الكامل: {citizen.FullName}\nالرقم القومي: {citizen.NationalId}\nتاريخ الميلاد: {citizen.DateOfBirth:dd/MM/yyyy}\nمحل الميلاد: {citizen.City}\nتاريخ التوثيق: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";

    var documents = new List<MockDocument>
    {
        new()
        {
            FileName = "بطاقة_الرقم_القومي_الرقمية.pdf",
            FileType = "application/pdf",
            FileBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(doc1Content))
        },
        new()
        {
            FileName = "رخصة_القيادة_الرقمية.pdf",
            FileType = "application/pdf",
            FileBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(doc2Content))
        },
        new()
        {
            FileName = "شهادة_الميلاد_الرقمية.pdf",
            FileType = "application/pdf",
            FileBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(doc3Content))
        }
    };

    return Results.Ok(new { success = true, data = documents });
});

app.MapPost("/api/external/portal/issue-document", (IssueDocumentRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.NationalId) || string.IsNullOrWhiteSpace(request.ServiceName))
    {
        return Results.BadRequest(new { success = false, message = "الرقم القومي واسم الخدمة مطلوبان" });
    }

    var citizen = mockCitizens.FirstOrDefault(c => c.NationalId == request.NationalId);
    if (citizen == null)
    {
        return Results.BadRequest(new { success = false, message = "المواطن غير مسجل في قاعدة بيانات بوابة مصر الرقمية" });
    }

    var docContent = $"بوابة مصر الرقمية - وثيقة إلكترونية موثقة وصادرة رسمياً\n" +
                     $"----------------------------------------------------\n" +
                     $"الرقم القومي: {citizen.NationalId}\n" +
                     $"الاسم الكامل: {citizen.FullName}\n" +
                     $"الخدمة: {request.ServiceName}\n" +
                     $"تاريخ الإصدار: {DateTime.Now:dd/MM/yyyy HH:mm:ss}\n" +
                     $"رقم المعاملة: {Guid.NewGuid()}\n" +
                     $"حالة المستند: موثق ومعتمد رسمياً من الجهات الحكومية المختصة.";

    var pdfBytes = BuildPdfBytes(docContent);

    var document = new MockDocument
    {
        FileName = $"مستند_رسمي_مستخرج_{request.ServiceName.Replace(" ", "_")}.pdf",
        FileType = "application/pdf",
        FileBase64 = Convert.ToBase64String(pdfBytes)
    };

    return Results.Ok(new 
    { 
        success = true, 
        transactionId = Guid.NewGuid().ToString(), 
        data = document,
        document = document 
    });
});

static byte[] BuildPdfBytes(string content)
{
    var lines = content.Split('\n');
    var streamBuilder = new System.Text.StringBuilder();
    streamBuilder.Append("BT\n/F1 12 Tf\n18 TL\n50 750 Td\n");
    foreach (var line in lines)
    {
        var escapedLine = line.Replace("(", "\\(").Replace(")", "\\)").Trim();
        streamBuilder.Append($"({escapedLine}) Tj T*\n");
    }
    streamBuilder.Append("ET");

    var streamContent = streamBuilder.ToString();
    var streamBytes = System.Text.Encoding.UTF8.GetBytes(streamContent);

    var objects = new System.Collections.Generic.List<string>
    {
        "%PDF-1.4\n",
        "1 0 obj\n<< /Type /Catalog /Pages 2 0 R >>\nendobj\n",
        "2 0 obj\n<< /Type /Pages /Kids [3 0 R] /Count 1 >>\nendobj\n",
        "3 0 obj\n<< /Type /Page /Parent 2 0 R /Resources << /Font << /F1 4 0 R >> >> /MediaBox [0 0 612 792] /Contents 5 0 R >>\nendobj\n",
        "4 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold >>\nendobj\n"
    };

    var offsets = new System.Collections.Generic.List<long>();
    long currentOffset = 0;

    var headerBytes = System.Text.Encoding.ASCII.GetBytes(objects[0]);
    currentOffset += headerBytes.Length;

    for (int i = 1; i <= 4; i++)
    {
        offsets.Add(currentOffset);
        currentOffset += System.Text.Encoding.UTF8.GetByteCount(objects[i]);
    }

    var streamHeader = $"5 0 obj\n<< /Length {streamBytes.Length} >>\nstream\n";
    var streamFooter = "\nendstream\nendobj\n";

    offsets.Add(currentOffset);
    currentOffset += System.Text.Encoding.UTF8.GetByteCount(streamHeader) + streamBytes.Length + System.Text.Encoding.UTF8.GetByteCount(streamFooter);

    using (var ms = new System.IO.MemoryStream())
    {
        ms.Write(headerBytes, 0, headerBytes.Length);
        for (int i = 1; i <= 4; i++)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(objects[i]);
            ms.Write(bytes, 0, bytes.Length);
        }

        var shBytes = System.Text.Encoding.UTF8.GetBytes(streamHeader);
        ms.Write(shBytes, 0, shBytes.Length);
        ms.Write(streamBytes, 0, streamBytes.Length);
        var sfBytes = System.Text.Encoding.UTF8.GetBytes(streamFooter);
        ms.Write(sfBytes, 0, sfBytes.Length);

        long xrefOffset = ms.Position;

        var xrefBuilder = new System.Text.StringBuilder();
        xrefBuilder.Append("xref\n0 6\n0000000000 65535 f \n");
        foreach (var offset in offsets)
        {
            xrefBuilder.Append($"{offset:D10} 00000 n \n");
        }
        xrefBuilder.Append($"trailer\n<< /Size 6 /Root 1 0 R >>\nstartxref\n{xrefOffset}\n%%EOF\n");

        var xrefBytes = System.Text.Encoding.ASCII.GetBytes(xrefBuilder.ToString());
        ms.Write(xrefBytes, 0, xrefBytes.Length);

        return ms.ToArray();
    }
}

// Data Models
public class MockCitizen
{
    public string NationalId { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string BuildingNumber { get; set; } = string.Empty;
    public string FloorNumber { get; set; } = string.Empty;
    public string ApartmentNumber { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
}

public class MockDocument
{
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public string FileBase64 { get; set; } = string.Empty;
}

public class SendOtpRequest
{
    public string NationalId { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}

public class VerifyOtpRequest
{
    public string NationalId { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Otp { get; set; } = string.Empty;
}

public class IssueDocumentRequest
{
    public string NationalId { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
}
