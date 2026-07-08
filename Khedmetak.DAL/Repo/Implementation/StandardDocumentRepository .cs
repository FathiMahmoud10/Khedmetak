using Khedmetak.Core.Data;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Khedmetak.DAL.Repositories
{
    public class StandardDocumentRepository : GenericRepository<StandardDocument>, IStandardDocumentRepository
    {
        public StandardDocumentRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<StandardDocument>> GetAllAsync()
            => await _dbSet.ToListAsync();

        public async Task<StandardDocument?> GetByIdWithUsagesAsync(int id)
            => await _dbSet
                .Include(s => s.RequiredDocuments)
                    .ThenInclude(r => r.GovService)
                .FirstOrDefaultAsync(s => s.Id == id);
    }
}