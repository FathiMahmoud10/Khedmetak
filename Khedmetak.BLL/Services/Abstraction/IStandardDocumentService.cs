// Khedmetak.BLL/Services/Interfaces/IStandardDocumentService.cs
using Khedmetak.BLL.DTOS.StandardDocument;
using Khedmetak.BLL.DTOS.StandardDocument.Khedmetak.DAL.DTOs.StandardDocument;
using Khedmetak.DAL.DTOs.StandardDocument;

namespace Khedmetak.BLL.Services.Interfaces
{
    public interface IStandardDocumentService
    {
        Task<IEnumerable<StandardDocumentResponseDto>> GetAllAsync();
        Task<StandardDocumentResponseDto?> GetByIdAsync(int id);
        Task<StandardDocumentResponseDto> CreateAsync(CreateStandardDocumentDto dto, string webRootPath);
        Task<bool> UpdateAsync(UpdateStandardDocumentDto dto, string webRootPath);
        Task<bool> DeleteAsync(int id, string webRootPath);
    }
}