using Khedmetak.Core.Data;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Khedmetak.DAL.Repositories
{
    public class GovServiceRepository : GenericRepository<GovService>, IGovServiceRepository
    {
        public GovServiceRepository(AppDbContext context) : base(context) { }

        public async Task<GovService?> GetServiceWithDetailsAsync(int id)
            => await _dbSet
                .Include(g => g.Category)
                .Include(g => g.ServiceSteps.OrderBy(s => s.StepOrder))
                .Include(g => g.RequiredDocuments)
                .Include(g => g.ServiceGeneralDocs)
                .Include(g => g.ServiceOptions)
                    .ThenInclude(o => o.ServiceOptionChoices)
                .FirstOrDefaultAsync(g => g.Id == id);

        public async Task<IEnumerable<GovService>> GetAllWithCategoryAsync()
            => await _dbSet.Include(g => g.Category).ToListAsync();

        public async Task<IEnumerable<GovService>> GetByCategoryAsync(int categoryId)
            => await _dbSet
                .Include(g => g.Category)
                .Where(g => g.CategoryId == categoryId)
                .ToListAsync();
    }
}