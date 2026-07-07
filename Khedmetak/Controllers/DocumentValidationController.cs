using Khedmetak.AI.Agents.Abstraction;
using Khedmetak.AI.DTOs.DocumentValidationDTO;
using Microsoft.AspNetCore.Mvc;

namespace Khedmetak.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentValidationController : ControllerBase
{
    private readonly IDocumentValidationService _documentValidationService;

    public DocumentValidationController(
        IDocumentValidationService documentValidationService)
    {
        _documentValidationService = documentValidationService;
    }

    [HttpPost("validate")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Validate(
    [FromForm] DocumentValidationRequest request)
    {
        if (request.Document == null || request.Document.Length == 0)
            return BadRequest("No file uploaded.");

        await using var stream = new MemoryStream();
        await request.Document.CopyToAsync(stream);

        byte[]? comparisonBytes = null;
        string? comparisonMediaType = null;

        if (request.ComparisonDocument is { Length: > 0 })
        {
            await using var comparisonStream = new MemoryStream();
            await request.ComparisonDocument.CopyToAsync(comparisonStream);
            comparisonBytes = comparisonStream.ToArray();
            comparisonMediaType = request.ComparisonDocument.ContentType;
        }

        var result = await _documentValidationService.ValidateAsync(
            stream.ToArray(),
            request.Document.ContentType,
            comparisonBytes,
            comparisonMediaType,
            request.ExpectedDocumentType,
            request.Rules);

        Console.WriteLine("==========");
        Console.WriteLine(result);
        Console.WriteLine("==========");

        return Ok(result);
    }
}