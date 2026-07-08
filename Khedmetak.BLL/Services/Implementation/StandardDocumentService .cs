// Khedmetak.BLL/Services/StandardDocumentService.cs
using Khedmetak.BLL.DTOS.StandardDocument;
using Khedmetak.BLL.DTOS.StandardDocument.Khedmetak.DAL.DTOs.StandardDocument;
using Khedmetak.BLL.Services.Interfaces;
using Khedmetak.Core.Data;
using Khedmetak.DAL.DTOs.StandardDocument;
using Khedmetak.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Khedmetak.BLL.Services
{
    public class StandardDocumentService : IStandardDocumentService
    {
        private readonly IStandardDocumentRepository _repo;
        private readonly AppDbContext _context;

        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf" };

        public StandardDocumentService(IStandardDocumentRepository repo, AppDbContext context)
        {
            _repo = repo;
            _context = context;
        }

        public async Task<IEnumerable<StandardDocumentResponseDto>> GetAllAsync()
        {
            var docs = await _repo.GetAllAsync();
            return docs.Select(MapToDto);
        }

        public async Task<StandardDocumentResponseDto?> GetByIdAsync(int id)
        {
            var doc = await _repo.GetByIdAsync(id);
            return doc is null ? null : MapToDto(doc);
        }

        public async Task<StandardDocumentResponseDto> CreateAsync(CreateStandardDocumentDto dto, string webRootPath)
        {
            var entity = new Khedmetak.DAL.Entities.StandardDocument
            {
                DocumentName = dto.DocumentName,
                GeneralRule = dto.GeneralRule,
                ImagePath = string.Empty
            };

            if (dto.StandardDocumentFile is not null && dto.StandardDocumentFile.Length > 0)
            {
                ValidateExtension(dto.StandardDocumentFile.FileName);
                entity.ImagePath = await SaveFileAsync(dto.StandardDocumentFile, webRootPath);
            }

            _repo.Add(entity);
            await _context.SaveChangesAsync();

            return MapToDto(entity);
        }

        public async Task<bool> UpdateAsync(UpdateStandardDocumentDto dto, string webRootPath)
        {
            var existing = await _repo.GetByIdAsync(dto.Id);
            if (existing is null) return false;

            existing.DocumentName = dto.DocumentName;
            existing.GeneralRule = dto.GeneralRule;

            if (dto.StandardDocumentFile is not null && dto.StandardDocumentFile.Length > 0)
            {
                ValidateExtension(dto.StandardDocumentFile.FileName);

                DeleteFileIfExists(existing.ImagePath, webRootPath);
                existing.ImagePath = await SaveFileAsync(dto.StandardDocumentFile, webRootPath);
            }

            _repo.Update(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string webRootPath)
        {
            var doc = await _repo.GetByIdAsync(id);
            if (doc is null) return false;

            DeleteFileIfExists(doc.ImagePath, webRootPath);

            _repo.Delete(doc);
            await _context.SaveChangesAsync();
            return true;
        }

        // ---------- Helpers ----------

        private static void ValidateExtension(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext))
                throw new InvalidOperationException("نوع الملف غير مسموح به. الأنواع المسموحة: jpg, jpeg, png, pdf");
        }

        private static async Task<string> SaveFileAsync(IFormFile file, string webRootPath)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var uploadsFolder = Path.Combine(webRootPath, "uploads", "standards");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueName = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(uploadsFolder, uniqueName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/standards/{uniqueName}";
        }

        private static void DeleteFileIfExists(string? imagePath, string webRootPath)
        {
            if (string.IsNullOrEmpty(imagePath)) return;

            var fullPath = Path.Combine(webRootPath, imagePath.TrimStart('/'));
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

        private static StandardDocumentResponseDto MapToDto(Khedmetak.DAL.Entities.StandardDocument doc)
            => new()
            {
                Id = doc.Id,
                DocumentName = doc.DocumentName,
                ImagePath = doc.ImagePath,
                GeneralRule = doc.GeneralRule
            };
    }
}