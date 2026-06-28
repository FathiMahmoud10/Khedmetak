// UserDocumentService.cs
using AutoMapper;
using Khedmetak.BLL.DTOS.UserDocument;
using Khedmetak.BLL.Services.Abstraction;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.Abstraction.UnitOfWork;
using Microsoft.Extensions.Configuration;
using System.Linq;

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
            _uploadsRoot = configuration["UploadsPath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        }

        // FIX: there is no AutoMapper Profile anywhere in the solution that registers
        // UserDocument -> UserDocumentDto, so _mapper.Map<UserDocumentDto>(...) always threw
        // "Missing type map configuration or unsupported mapping" — the file itself uploaded
        // fine and was saved to the DB, but the API call still failed on this last step.
        // Mapping this one DTO by hand removes the dependency on a Profile that doesn't exist,
        // without touching AutoMapper setup used elsewhere in the project.
        private static UserDocumentDto ToDto(UserDocument entity)
        {
            return new UserDocumentDto
            {
                Id = entity.Id,
                FileName = entity.FileName,
                FilePath = entity.FilePath,
                FileType = entity.FileType,
                ValidationStatus = entity.ValidationStatus,
                UploadedAt = entity.UploadedAt,
                UserId = entity.UserId,
                ChatSessionId = entity.ChatSessionId,
                RequiredDocumentId = entity.RequiredDocumentId,
            };
        }

        public async Task<IEnumerable<UserDocumentDto>> GetUserDocumentsAsync(int userId)
        {
            var docs = await _unitOfWork.UserDocuments.GetByUserIdAsync(userId);
            return docs.Select(ToDto);
        }

        public async Task<UserDocumentDto> UploadDocumentAsync(int userId, UploadDocumentDto dto)
        {
            var userFolder = Path.Combine(_uploadsRoot, userId.ToString());
            Directory.CreateDirectory(userFolder);

            var uniqueName = $"{Guid.NewGuid()}_{dto.File.FileName}";
            var fullPath = Path.Combine(userFolder, uniqueName);
            // FIX: build the public/relative path with forward slashes explicitly instead of
            // Path.Combine, which uses '\' on Windows and breaks the URL the frontend builds
            // for previewing/downloading the file (no leading slash + backslashes).
            var relativePath = $"/uploads/{userId}/{uniqueName}";

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

            return ToDto(entity);
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