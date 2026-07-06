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
    public class AdminImportantNotesController : ControllerBase
    {
        private readonly IGovServiceAdminService _adminService;
        public AdminImportantNotesController(IGovServiceAdminService adminService) => _adminService = adminService;

        [HttpGet("{id:int}/important-notes")]
        public async Task<IActionResult> GetImportantNotes(int id)
        {
            var notes = await _adminService.GetImportantNotesAsync(id);
            return Ok(ApiResponse<object>.Ok(notes));
        }

        [HttpPost("{id:int}/important-notes")]
        public async Task<IActionResult> AddImportantNote(int id, [FromBody] CreateServiceImportantNoteDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ApiResponse<object>.Fail("Invalid input data."));
            var note = await _adminService.AddImportantNoteAsync(id, dto);
            if (note is null) return NotFound(ApiResponse<object>.Fail($"Service with id {id} not found."));
            return Ok(ApiResponse<object>.Ok(note, "Important note added successfully."));
        }

        [HttpPut("{id:int}/important-notes/{noteId:int}")]
        public async Task<IActionResult> UpdateImportantNote(int id, int noteId, [FromBody] UpdateServiceImportantNoteDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ApiResponse<object>.Fail("Invalid input data."));
            var note = await _adminService.UpdateImportantNoteAsync(id, noteId, dto);
            if (note is null) return NotFound(ApiResponse<object>.Fail($"Note with id {noteId} not found for service {id}."));
            return Ok(ApiResponse<object>.Ok(note, "Important note updated successfully."));
        }

        [HttpDelete("{id:int}/important-notes/{noteId:int}")]
        public async Task<IActionResult> DeleteImportantNote(int id, int noteId)
        {
            var deleted = await _adminService.DeleteImportantNoteAsync(id, noteId);
            if (!deleted) return NotFound(ApiResponse<object>.Fail($"Note with id {noteId} not found for service {id}."));
            return Ok(ApiResponse<object>.Ok(null!, "Important note deleted successfully."));
        }
    }
}
