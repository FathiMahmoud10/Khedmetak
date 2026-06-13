using Khedmetak.Core.Data;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Khedmetak.DAL.Repositories
{
    public class ServiceStepRepository : GenericRepository<ServiceSteps>, IServiceStepRepository
    {
        public ServiceStepRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<ServiceSteps>> GetByServiceIdAsync(int govServiceId)
            => await _dbSet
                .Where(s => s.GovServiceId == govServiceId)
                .OrderBy(s => s.StepOrder)
                .ToListAsync();
    }
}
