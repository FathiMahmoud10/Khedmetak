using Khedmetak.BLL.ApiResponse;
using Khedmetak.BLL.DTOS.Documents;
using Khedmetak.BLL.DTOS.UploadDocument.Khedmetak.BLL.DTOS.Documents;
using Khedmetak.BLL.Services.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Khedmetak.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentsController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

 
        private int GetCurrentUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] UploadDocumentDto dto)
        {
            var userId = GetCurrentUserId(); 

            var (success, message, data) = await _documentService.UploadDocumentAsync(dto, userId);

            if (!success)
                return BadRequest(ApiResponse<string>.Fail(message));

            return Ok(ApiResponse<UserDocumentDto>.Ok(data!));
        }

        [HttpGet("my-documents")]
        public async Task<IActionResult> GetMyDocuments()
        {
            var userId = GetCurrentUserId();
            var docs = await _documentService.GetUserDocumentsAsync(userId);
            return Ok(ApiResponse<IEnumerable<UserDocumentDto>>.Ok(docs));
        }
    }
}