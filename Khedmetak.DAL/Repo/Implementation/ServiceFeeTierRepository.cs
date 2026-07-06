using Khedmetak.Core.Data;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Khedmetak.DAL.Repositories
{
    public class ServiceFeeTierRepository : GenericRepository<ServiceFeeTier>, IServiceFeeTierRepository
    {
        public ServiceFeeTierRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<ServiceFeeTier>> GetByServiceIdAsync(int govServiceId)
            => await _dbSet
                .Where(t => t.GovServiceId == govServiceId)
                .OrderBy(t => t.DisplayOrder)
                .ToListAsync();
    }
}
