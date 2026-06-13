using Khedmetak.DAL.Enums;

namespace Khedmetak.BLL.DTOS.Admin
{

    public class RequiredDocumentAdminDto
    {
        public int Id { get; set; }
        public string DocumentName { get; set; } = string.Empty;
        public bool IsMandatory { get; set; }
        public DocumentType DocumentType { get; set; }
    }
    public class CreateRequiredDocumentDto
    {
        public string DocumentName { get; set; } = string.Empty;
        public bool IsMandatory { get; set; }
        public DocumentType DocumentType { get; set; }
    }

    public class UpdateRequiredDocumentDto
    {
        public string DocumentName { get; set; } = string.Empty;
        public bool IsMandatory { get; set; }
        public DocumentType DocumentType { get; set; }
    }


}
