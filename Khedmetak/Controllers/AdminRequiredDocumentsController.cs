using Khedmetak.BLL.ApiResponse;
using Khedmetak.BLL.DTOS.Admin;
using Khedmetak.BLL.Services.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Khedmetak.API.Controllers
{
    [ApiController]
    [Route("api/admin/govservices")]
    [Authorize(Roles = "Admin")]
    public class AdminRequiredDocumentsController : ControllerBase
    {
        private readonly IGovServiceAdminService _adminService;
        public AdminRequiredDocumentsController(IGovServiceAdminService adminService) => _adminService = adminService;

        [HttpGet("{id:int}/required-documents")]
        public async Task<IActionResult> GetRequiredDocuments(int id)
        {
            var docs = await _adminService.GetRequiredDocumentsAsync(id);
            return Ok(ApiResponse<object>.Ok(docs));
        }

        [HttpPost("{id:int}/required-documents")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddRequiredDocument(int id, [FromForm] CreateRequiredDocumentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ApiResponse<object>.Fail("Invalid input data."));
            var doc = await _adminService.AddRequiredDocumentAsync(id, dto);
            if (doc is null) return NotFound(ApiResponse<object>.Fail($"Service with id {id} not found."));
            return Ok(ApiResponse<object>.Ok(doc, "Required document added successfully."));
        }

        [HttpPut("{id:int}/required-documents/{docId:int}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateRequiredDocument(int id, int docId, [FromForm] UpdateRequiredDocumentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ApiResponse<object>.Fail("Invalid input data."));
            var doc = await _adminService.UpdateRequiredDocumentAsync(id, docId, dto);
            if (doc is null) return NotFound(ApiResponse<object>.Fail($"Document with id {docId} not found for service {id}."));
            return Ok(ApiResponse<object>.Ok(doc, "Required document updated successfully."));
        }

        [HttpDelete("{id:int}/required-documents/{docId:int}")]
        public async Task<IActionResult> DeleteRequiredDocument(int id, int docId)
        {
            var deleted = await _adminService.DeleteRequiredDocumentAsync(id, docId);
            if (!deleted) return NotFound(ApiResponse<object>.Fail($"Document with id {docId} not found for service {id}."));
            return Ok(ApiResponse<object>.Ok(null!, "Required document deleted successfully."));
        }
    }
}
