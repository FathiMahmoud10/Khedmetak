using Khedmetak.BLL.ApiResponse;
using Khedmetak.BLL.DTOS.Admin;
using Khedmetak.BLL.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace Khedmetak.API.Controllers
{
    // --Added by Naglaa

    [ApiController]
    [Route("api/admin/govservices")]
    public class AdminServicesController : ControllerBase
    {
        private readonly IGovServiceAdminService _adminService;
        private readonly IGovServiceService _govServiceService;

        public AdminServicesController(
            IGovServiceAdminService adminService,
            IGovServiceService govServiceService)
        {
            _adminService = adminService;
            _govServiceService = govServiceService;
        }
        /*   استيراد خدمات + خطوات  من ملف اكسيل  */
        [HttpPost("import-excel")]
        public async Task<IActionResult> ImportFromExcel(IFormFile file)
        {
            if (file is null || file.Length == 0)
                return BadRequest(ApiResponse<object>.Fail("الرجاء اختيار ملف Excel صحيح."));

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != ".xlsx" && extension != ".xls")
                return BadRequest(ApiResponse<object>.Fail("نوع الملف غير مدعوم، الرجاء رفع ملف .xlsx أو .xls."));

            try
            {
                await using var stream = file.OpenReadStream();
                var result = await _adminService.ImportServicesFromExcelAsync(stream);

                var message = result.Errors.Count == 0
                    ? "تم استيراد البيانات بنجاح."
                    : $"تم الاستيراد مع وجود {result.Errors.Count} صف به أخطاء.";

                return Ok(ApiResponse<object>.Ok(result, message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail($"تعذّر قراءة ملف الإكسل: {ex.Message}"));
            }
        }

        /*  عرض تفاصيل خدمة كاملة (الرسوم + الخطوات + المستندات + الخيارات) */
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var service = await _govServiceService.GetServiceDetailsAsync(id);
            if (service is null)
                return NotFound(ApiResponse<object>.Fail($"Service with id {id} not found."));

            return Ok(ApiResponse<object>.Ok(service));
        }


        /*  إضافة خدمة حكومية جديدة  */
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGovServiceDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.Fail("Invalid input data."));

            var created = await _adminService.CreateServiceAsync(dto);
            return CreatedAtAction(
                actionName: nameof(GetById),
                routeValues: new { id = created.Id },
                value: ApiResponse<object>.Ok(created, "Service created successfully."));
        }

        /*  تعديل خدمة موجودة 
         *  (الاسم / الوصف / المدة / التصنيف) */
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateGovServiceDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.Fail("Invalid input data."));

            var updated = await _adminService.UpdateServiceAsync(id, dto);
            if (updated is null)
                return NotFound(ApiResponse<object>.Fail($"Service with id {id} not found."));

            return Ok(ApiResponse<object>.Ok(updated, "Service updated successfully."));
        }

        /*  حذف خدمة  */
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _adminService.DeleteServiceAsync(id);
            if (!deleted)
                return NotFound(ApiResponse<object>.Fail($"Service with id {id} not found."));

            return Ok(ApiResponse<object>.Ok(null!, "Service deleted successfully."));
        }


    }
}
