using Khedmetak.DAL.Enums;

namespace Khedmetak.BLL.DTOS.Admin
{
    public class StandardDocumentAdminDto
    {
        public int Id { get; set; }
        public string DocumentName { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string? GeneralRule { get; set; }
    }

    public class RequiredDocumentAdminDto
    {
        public int Id { get; set; }
        public string DocumentName { get; set; } = string.Empty;
        public bool IsMandatory { get; set; }
        public DocumentType DocumentType { get; set; }
        public int GovServiceId { get; set; }
        public StandardDocumentAdminDto? StandardDocument { get; set; }
    }

    public class CreateRequiredDocumentDto
    {
        public string DocumentName { get; set; } = string.Empty;
        public bool IsMandatory { get; set; }
        public DocumentType DocumentType { get; set; }
        public int GovServiceId { get; set; }
        public int? StandardDocumentId { get; set; }
    }

    public class UpdateRequiredDocumentDto
    {
        public int Id { get; set; }
        public string DocumentName { get; set; } = string.Empty;
        public bool IsMandatory { get; set; }
        public DocumentType DocumentType { get; set; }
        public int? StandardDocumentId { get; set; }
    }
}