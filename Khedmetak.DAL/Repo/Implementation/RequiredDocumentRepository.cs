using Khedmetak.Core.Data;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repositories;
using Khedmetak.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Khedmetak.DAL.Repo.Implementation
{
    public class RequiredDocumentRepository : GenericRepository<RequiredDocument>, IRequiredDocumentRepository
    {
        public RequiredDocumentRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<RequiredDocument>> GetByServiceIdAsync(int govServiceId)
            => await _dbSet
                .Where(d => d.GovServiceId == govServiceId)
                .ToListAsync();
    }
}
