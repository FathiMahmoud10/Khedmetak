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
            return Ok(categories);
        }
    }
}