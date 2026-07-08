// Khedmetak.API/Controllers/StandardDocumentsController.cs
using Khedmetak.BLL.DTOS.StandardDocument;
using Khedmetak.BLL.DTOS.StandardDocument.Khedmetak.DAL.DTOs.StandardDocument;
using Khedmetak.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Khedmetak.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StandardDocumentsController : ControllerBase
    {
        private readonly IStandardDocumentService _service;
        private readonly IWebHostEnvironment _env;

        public StandardDocumentsController(IStandardDocumentService service, IWebHostEnvironment env)
        {
            _service = service;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateStandardDocumentDto dto)
        {
            try
            {
                var result = await _service.CreateAsync(dto, _env.WebRootPath);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateStandardDocumentDto dto)
        {
            if (id != dto.Id) return BadRequest();

            try
            {
                var success = await _service.UpdateAsync(dto, _env.WebRootPath);
                return success ? NoContent() : NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id, _env.WebRootPath);
            return success ? NoContent() : NotFound();
        }
    }
}