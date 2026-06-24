using Khedmetak.BLL.DTOS.UserDocument;

namespace Khedmetak.BLL.Services.Abstraction
{
    public interface IUserDocumentService
    {
        Task<IEnumerable<UserDocumentDto>> GetUserDocumentsAsync(int userId);
        Task<UserDocumentDto> UploadDocumentAsync(int userId, UploadDocumentDto dto);
        Task<bool> DeleteDocumentAsync(int documentId, int userId);
    }
}