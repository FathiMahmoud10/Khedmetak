using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Khedmetak.AI.DTOs.ImageRag;
using Khedmetak.AI.Services.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Khedmetak.Controllers
{
    [ApiController]
    [Route("api/admin/image-rag")]
    public class AdminImageRagController : ControllerBase
    {
        private readonly IImageVectorDbService _vectorDbService;
        private readonly ILogger<AdminImageRagController> _logger;
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

        public AdminImageRagController(
            IImageVectorDbService vectorDbService,
            ILogger<AdminImageRagController> logger)
        {
            _vectorDbService = vectorDbService;
            _logger = logger;
        }


        /// Add a new document (image embedding + metadata).

        [HttpPost("AddImage")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddDocument([FromForm] UploadDocumentRequest request)
        {
            _logger.LogInformation("Admin CRUD: Starting AddDocument request for '{DocumentName}'", request?.DocumentName);

            if (request == null)
            {
                return BadRequest(new { success = false, message = "Request body is null." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Image validations
            if (request.Image == null || request.Image.Length == 0)
            {
                return BadRequest(new { success = false, message = "Empty image: Please upload a valid image file." });
            }

            var extension = Path.GetExtension(request.Image.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                return BadRequest(new { success = false, message = "Invalid image: Only standard image formats (JPG, PNG, BMP, GIF, WEBP) are allowed." });
            }

            try
            {
                await using var stream = request.Image.OpenReadStream();
                Console.WriteLine("=============== Before add");
                await _vectorDbService.AddDocumentAsync(request.DocumentName, stream, request.Image.FileName);
                Console.WriteLine("============== After add");
                return StatusCode(StatusCodes.Status201Created, new UploadDocumentResponse
                {
                    Success = true,
                    FileName = request.Image.FileName,
                    Message = "Document added successfully."
                });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Duplicate DocumentName"))
            {
                _logger.LogWarning(ex, "Duplicate DocumentName: '{DocumentName}' already exists.", request.DocumentName);
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errors: Failed to add document '{DocumentName}' due to server error.", request.DocumentName);
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "Qdrant unavailable or search failure." });
            }
        }

        /// Update an existing document (image only, documentName only, or both).
 
        [HttpPut("{documentName}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateDocument(string documentName, [FromForm] UpdateDocumentRequest request)
        {
            _logger.LogInformation("Admin CRUD: Starting UpdateDocument request for '{DocumentName}'", documentName);

            if (string.IsNullOrWhiteSpace(documentName))
            {
                return BadRequest(new { success = false, message = "DocumentName in path cannot be empty." });
            }

            if (request == null || (request.Image == null && string.IsNullOrWhiteSpace(request.DocumentName)))
            {
                return BadRequest(new { success = false, message = "You must provide either a new Image or a new DocumentName to update." });
            }

            Stream? imageStream = null;
            string? fileName = null;

            if (request.Image != null)
            {
                if (request.Image.Length == 0)
                {
                    return BadRequest(new { success = false, message = "Empty image: Uploaded image file is empty." });
                }

                var extension = Path.GetExtension(request.Image.FileName).ToLowerInvariant();
                if (!AllowedExtensions.Contains(extension))
                {
                    return BadRequest(new { success = false, message = "Invalid image: Only standard image formats are allowed." });
                }

                imageStream = request.Image.OpenReadStream();
                fileName = request.Image.FileName;
            }

            try
            {
                await _vectorDbService.UpdateDocumentAsync(documentName, request.DocumentName, imageStream, fileName);

                if (imageStream != null)
                {
                    await imageStream.DisposeAsync();
                }

                return Ok(new { success = true, message = "Document updated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Update failures: Document '{DocumentName}' not found.", documentName);
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Duplicate DocumentName"))
            {
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update failures: Server error during update for '{DocumentName}'.", documentName);
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "Qdrant unavailable or update failure." });
            }
            finally
            {
                if (imageStream != null)
                {
                    await imageStream.DisposeAsync();
                }
            }
        }

        /// Delete a document by its documentName.
  
        [HttpDelete("{documentName}")]
        public async Task<IActionResult> DeleteDocument(string documentName)
        {
            _logger.LogInformation("Admin CRUD: Starting DeleteDocument request for '{DocumentName}'", documentName);

            if (string.IsNullOrWhiteSpace(documentName))
            {
                return BadRequest(new { success = false, message = "DocumentName cannot be empty." });
            }

            try
            {
                await _vectorDbService.DeleteDocumentAsync(documentName);
                return Ok(new { success = true, message = "Document deleted successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Delete failures: Document '{DocumentName}' not found.", documentName);
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete failures: Server error during delete of '{DocumentName}'.", documentName);
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "Qdrant unavailable or delete failure." });
            }
        }

       
        //[HttpGet("{documentName}")]
        //public async Task<IActionResult> GetDocument(string documentName)
        //{
        //    _logger.LogInformation("Admin CRUD: Starting GetDocument request for '{DocumentName}'", documentName);

        //    if (string.IsNullOrWhiteSpace(documentName))
        //    {
        //        return BadRequest(new { success = false, message = "DocumentName cannot be empty." });
        //    }

        //    try
        //    {
        //        var doc = await _vectorDbService.GetDocumentAsync(documentName);
        //        if (doc == null)
        //        {
        //            _logger.LogWarning("Get Document: Document '{DocumentName}' not found.", documentName);
        //            return NotFound(new { success = false, message = $"Document '{documentName}' not found." });
        //        }

        //        return Ok(doc);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Errors: Failed to retrieve document '{DocumentName}' metadata.", documentName);
        //        return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "Qdrant unavailable or search failure." });
        //    }
        //}


        [HttpGet]
        public async Task<IActionResult> GetAllDocuments()
        {
            _logger.LogInformation("Admin CRUD: Starting GetAllDocuments request.");

            try
            {
                var (documents, totalCount) = await _vectorDbService.GetAllDocumentsAsync();

                return Ok(new
                {
                    totalCount,
                    documents
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errors: Failed to list documents.");

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "Qdrant unavailable or search failure."
                });
            }
        }
    }
}
