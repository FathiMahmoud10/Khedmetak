using AutoMapper;
using Khedmetak.BLL.DTOS.Admin;
using Khedmetak.BLL.DTOS.Categorys;
using Khedmetak.BLL.Services.Abstraction;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.Abstraction.UnitOfWork;
using Khedmetak.DAL.Repositories.Interfaces;

namespace Khedmetak.BLL.Services.Implementation
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper , IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllWithServicesCountAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
        {
            var entity = _mapper.Map<Category>(dto);

            _categoryRepository.Add(entity);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CategoryDto>(entity);
        }

        public async Task<CategoryDto?> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
        {
            var entity = await _categoryRepository.GetByIdAsync(id);
            if (entity is null) return null;

            entity.Name = dto.Name;

            _categoryRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CategoryDto>(entity);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var entity = await _categoryRepository.GetByIdAsync(id);
            if (entity is null) return false;

            _categoryRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
