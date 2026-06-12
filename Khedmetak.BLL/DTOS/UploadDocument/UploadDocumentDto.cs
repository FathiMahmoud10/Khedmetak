using Microsoft.AspNetCore.Http;

namespace Khedmetak.BLL.DTOS.UploadDocument.Khedmetak.BLL.DTOS.Documents
{
    public class UploadDocumentDto
    {
        public int? RequiredDocumentId { get; set; }
        public IFormFile File { get; set; } = null!;
    }
}
