using Khedmetak.BLL.ApiResponse;
using Khedmetak.BLL.DTOS.Categorys;
using Khedmetak.BLL.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace Khedmetak.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /*عرض كل الاقسام */
        // -- Added By Fathi 
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();

            /*
             * وظيغة الجزء اللي تحت اني بوحد الريسبونس لل 
             * API 
             * بشكل محدد 
             */
            return Ok(ApiResponse<IEnumerable<CategoryDto>>.Ok(categories));
        }
    }
}