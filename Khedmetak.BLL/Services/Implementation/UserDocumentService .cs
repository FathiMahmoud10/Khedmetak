using AutoMapper;
using Khedmetak.BLL.DTOS.UserDocument;
using Khedmetak.BLL.Services.Abstraction;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.Abstraction.UnitOfWork;
using Microsoft.Extensions.Configuration;

namespace Khedmetak.BLL.Services.Implementation
{
    public class UserDocumentService : IUserDocumentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly string _uploadsRoot;

        public UserDocumentService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _uploadsRoot = configuration["UploadsPath"]
                ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        }

        public async Task<IEnumerable<UserDocumentDto>> GetUserDocumentsAsync(int userId)
        {
            var docs = await _unitOfWork.UserDocuments.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<UserDocumentDto>>(docs);
        }

        public async Task<UserDocumentDto> UploadDocumentAsync(int userId, UploadDocumentDto dto)
        {
            var userFolder = Path.Combine(_uploadsRoot, userId.ToString());
            Directory.CreateDirectory(userFolder);

            var uniqueName = $"{Guid.NewGuid()}_{dto.File.FileName}";
            var fullPath = Path.Combine(userFolder, uniqueName);
            var relativePath = Path.Combine("uploads", userId.ToString(), uniqueName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
                await dto.File.CopyToAsync(stream);

            var entity = new UserDocument
            {
                UserId = userId,
                FileName = dto.File.FileName,
                FilePath = relativePath,
                FileType = dto.File.ContentType,
                UploadedAt = DateTime.UtcNow,
                ValidationStatus = "Pending",
                ChatSessionId = dto.ChatSessionId,
                RequiredDocumentId = dto.RequiredDocumentId,
            };

            _unitOfWork.UserDocuments.Add(entity);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserDocumentDto>(entity);
        }

        public async Task<bool> DeleteDocumentAsync(int documentId, int userId)
        {
            var doc = await _unitOfWork.UserDocuments.GetByIdAsync(documentId);
            if (doc is null || doc.UserId != userId) return false;

            var fullPath = Path.Combine(_uploadsRoot, doc.UserId.ToString(),
                           Path.GetFileName(doc.FilePath));
            if (File.Exists(fullPath)) File.Delete(fullPath);

            _unitOfWork.UserDocuments.Delete(doc);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}