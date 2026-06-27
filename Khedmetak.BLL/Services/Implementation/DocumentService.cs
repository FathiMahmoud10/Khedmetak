// Khedmetak.BLL/Services/Implementation/DocumentService.cs
using AutoMapper;
using Khedmetak.BLL.DTOS.Documents;
using Khedmetak.BLL.DTOS.UploadDocument.Khedmetak.BLL.DTOS.Documents;
using Khedmetak.BLL.Services.Abstraction;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.Abstraction.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace Khedmetak.BLL.Services.Implementation
{
    public class DocumentService : IDocumentService
    {
        private const long MaxFileSizeBytes = 5 * 1024 * 1024;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly string _webRootPath;

        public DocumentService(IUnitOfWork unitOfWork, IMapper mapper, IHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _webRootPath = Path.Combine(env.ContentRootPath, "wwwroot");
        }

        public async Task<(bool Success, string Message, UserDocumentDto? Data)>
            UploadDocumentAsync(UploadDocumentDto dto, int userId)
        {
            var file = dto.File;

            if (file == null || file.Length == 0)
                return (false, "الملف فاضي أو مش موجود", null);

            if (file.Length > MaxFileSizeBytes)
                return (false, "حجم الملف أكبر من 5 MB", null);

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            var uploadsFolder = Path.Combine(_webRootPath, "uploads", "documents");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueName = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(uploadsFolder, uniqueName);

            await using (var stream = new FileStream(fullPath, FileMode.Create))
                await file.CopyToAsync(stream);

            var entity = new UserDocument
            {
                UserId = userId,
                RequiredDocumentId = dto.RequiredDocumentId,
                FileName = file.FileName,
                FilePath = $"/uploads/documents/{uniqueName}",
                FileType = ext,
                UploadedAt = DateTime.UtcNow,
                ValidationStatus = "Pending"
            };

            _unitOfWork.UserDocuments.Add(entity);
            await _unitOfWork.SaveChangesAsync();

            return (true, "تم رفع الملف بنجاح", _mapper.Map<UserDocumentDto>(entity));
        }

        // ✅ int? بدل int
        public async Task<bool> SaveUserDocumentsAsync(List<IFormFile> files, int userId, int? chatSessionId)
        {
            try
            {
                var uploadsFolder = Path.Combine(_webRootPath, "uploads", "documents");
                Directory.CreateDirectory(uploadsFolder);

                foreach (var file in files)
                {
                    if (file.Length == 0 || file.Length > MaxFileSizeBytes) continue;

                    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                    var uniqueName = $"{Guid.NewGuid()}{ext}";
                    var fullPath = Path.Combine(uploadsFolder, uniqueName);

                    await using (var stream = new FileStream(fullPath, FileMode.Create))
                        await file.CopyToAsync(stream);

                    var entity = new UserDocument
                    {
                        UserId = userId,
                        ChatSessionId = chatSessionId,  // ✅ int? nullable
                        FileName = file.FileName,
                        FilePath = $"/uploads/documents/{uniqueName}",
                        FileType = ext,
                        UploadedAt = DateTime.UtcNow,
                        ValidationStatus = "Pending"
                    };

                    _unitOfWork.UserDocuments.Add(entity);
                }

                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<UserDocumentDto>> GetUserDocumentsAsync(int userId)
        {
            var docs = await _unitOfWork.UserDocuments.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<UserDocumentDto>>(docs);
        }
    }
}