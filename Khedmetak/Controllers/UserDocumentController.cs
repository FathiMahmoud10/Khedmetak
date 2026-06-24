using Khedmetak.BLL.ApiResponse;
using Khedmetak.BLL.DTOS.UserDocument;
using Khedmetak.BLL.Services.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Khedmetak.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserDocumentController : ControllerBase
    {
        private readonly IUserDocumentService _docService;

        public UserDocumentController(IUserDocumentService docService)
        {
            _docService = docService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> GetMyDocuments()
        {
            var docs = await _docService.GetUserDocumentsAsync(GetUserId());
            return Ok(ApiResponse<IEnumerable<UserDocumentDto>>.Ok(docs));
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromForm] UploadDocumentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Fail("بيانات غير صحيحة"));

            var result = await _docService.UploadDocumentAsync(GetUserId(), dto);
            return Ok(ApiResponse<UserDocumentDto>.Ok(result));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _docService.DeleteDocumentAsync(id, GetUserId());
            if (!deleted)
                return NotFound(ApiResponse<string>.Fail("المستند غير موجود أو لا تملك صلاحية حذفه"));

            return Ok(ApiResponse<string>.Ok("تم حذف المستند بنجاح"));
        }
    }
}