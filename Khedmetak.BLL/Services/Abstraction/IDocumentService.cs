using Khedmetak.BLL.DTOS.Documents;
using Khedmetak.BLL.DTOS.UploadDocument.Khedmetak.BLL.DTOS.Documents;
using Microsoft.AspNetCore.Http;

namespace Khedmetak.BLL.Services.Abstraction
{
    public interface IDocumentService
    {
        Task<(bool Success, string Message, UserDocumentDto? Data)> UploadDocumentAsync(UploadDocumentDto dto, int userId);
        Task<IEnumerable<UserDocumentDto>> GetUserDocumentsAsync(int userId);
        Task<bool> SaveUserDocumentsAsync(List<IFormFile> files, int userId, int? chatSessionId); // ✅ int? بدل int
    }
}