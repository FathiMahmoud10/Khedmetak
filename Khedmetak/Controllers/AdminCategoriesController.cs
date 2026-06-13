using Khedmetak.BLL.ApiResponse;
using Khedmetak.BLL.DTOS.Admin;
using Khedmetak.BLL.DTOS.Categorys;
using Khedmetak.BLL.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace Khedmetak.API.Controllers
{
    [ApiController]
    [Route("api/admin/categories")]
    public class AdminCategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public AdminCategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        /*  عرض كل التصنيفات */
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(ApiResponse<IEnumerable<CategoryDto>>.Ok(categories));
        }
        /*  إضافة تصنيف جديد  */
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.Fail("Invalid input data."));

            var created = await _categoryService.CreateCategoryAsync(dto);
            return CreatedAtAction(
                actionName: nameof(GetAll),
                value: ApiResponse<CategoryDto>.Ok(created, "Category created successfully."));
        }

        /*  تعديل تصنيف موجود  */
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.Fail("Invalid input data."));

            var updated = await _categoryService.UpdateCategoryAsync(id, dto);
            if (updated is null)
                return NotFound(ApiResponse<object>.Fail($"Category with id {id} not found."));

            return Ok(ApiResponse<CategoryDto>.Ok(updated, "Category updated successfully."));
        }

        /*  حذف تصنيف  */
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _categoryService.DeleteCategoryAsync(id);
            if (!deleted)
                return NotFound(ApiResponse<object>.Fail($"Category with id {id} not found."));

            return Ok(ApiResponse<object>.Ok(null!, "Category deleted successfully."));
        }

       
    }
}
