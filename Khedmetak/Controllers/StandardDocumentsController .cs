using Khedmetak.Core.Data;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Khedmetak.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StandardDocumentsController : ControllerBase
    {
        private readonly IStandardDocumentRepository _repo;
        private readonly AppDbContext _context; 

        public StandardDocumentsController(IStandardDocumentRepository repo, AppDbContext context)
        {
            _repo = repo;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _repo.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var doc = await _repo.GetByIdWithUsagesAsync(id);
            return doc is null ? NotFound() : Ok(doc);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StandardDocument dto)
        {
            _repo.Add(dto);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] StandardDocument dto)
        {
            if (id != dto.Id) return BadRequest();

            var existing = await _repo.GetByIdAsync(id);
            if (existing is null) return NotFound();

            _repo.Update(dto);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var doc = await _repo.GetByIdAsync(id);
            if (doc is null) return NotFound();

            _repo.Delete(doc);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}