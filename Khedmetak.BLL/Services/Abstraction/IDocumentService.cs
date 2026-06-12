using Khedmetak.BLL.DTOS.Documents;
using Khedmetak.BLL.DTOS.UploadDocument.Khedmetak.BLL.DTOS.Documents;

namespace Khedmetak.BLL.Services.Abstraction
{
    public interface IDocumentService
    {
        Task<(bool Success, string Message, UserDocumentDto? Data)> UploadDocumentAsync(UploadDocumentDto dto, int userId);
        Task<IEnumerable<UserDocumentDto>> GetUserDocumentsAsync(int userId);
    }
}