// Khedmetak.BLL/DTOS/StandardDocument/UpdateStandardDocumentDto.cs
using Microsoft.AspNetCore.Http;

namespace Khedmetak.BLL.DTOS.StandardDocument
{
    public class UpdateStandardDocumentDto
    {
        public int Id { get; set; }
        public string DocumentName { get; set; } = string.Empty;
        public string? GeneralRule { get; set; }
        public IFormFile? StandardDocumentFile { get; set; }   // اختياري - لو عايز يغيّر الصورة
    }
}