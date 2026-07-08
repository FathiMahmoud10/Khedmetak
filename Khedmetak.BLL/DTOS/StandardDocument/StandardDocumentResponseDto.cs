// Khedmetak.DAL/DTOs/StandardDocument/StandardDocumentResponseDto.cs
namespace Khedmetak.DAL.DTOs.StandardDocument
{
    public class StandardDocumentResponseDto
    {
        public int Id { get; set; }
        public string DocumentName { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string? GeneralRule { get; set; }
    }
}