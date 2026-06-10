using Khedmetak.BLL.DTOS.Admin;
using Khedmetak.BLL.DTOS.Categorys;

namespace Khedmetak.BLL.Services.Abstraction
{
    public interface ICategoryService
    {
        // ─── Public (read-only) ────────────────────────────────────
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();

        // ─── Admin (CRUD) ──────────────────────────────────────────
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto);
        Task<CategoryDto?> UpdateCategoryAsync(int id, UpdateCategoryDto dto);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
