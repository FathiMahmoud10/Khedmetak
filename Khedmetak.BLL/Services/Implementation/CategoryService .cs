using AutoMapper;
using Khedmetak.BLL.DTOS.Admin;
using Khedmetak.BLL.DTOS.Categorys;
using Khedmetak.BLL.Services.Abstraction;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.UnitOfWork;

namespace Khedmetak.BLL.Services.Implementation
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // ─── Public (read-only) ────────────────────────────────────

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllWithServicesCountAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        // ─── Admin (CRUD) ──────────────────────────────────────────

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
        {
            var entity = _mapper.Map<Category>(dto);
            _unitOfWork.Categories.Add(entity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<CategoryDto>(entity);
        }

        public async Task<CategoryDto?> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
        {
            var entity = await _unitOfWork.Categories.GetByIdAsync(id);
            if (entity is null) return null;

            _mapper.Map(dto, entity);
            _unitOfWork.Categories.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<CategoryDto>(entity);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var entity = await _unitOfWork.Categories.GetByIdAsync(id);
            if (entity is null) return false;

            _unitOfWork.Categories.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
