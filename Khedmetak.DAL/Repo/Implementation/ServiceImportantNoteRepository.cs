using Khedmetak.Core.Data;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Khedmetak.DAL.Repositories
{
    public class ServiceImportantNoteRepository : GenericRepository<ServiceImportantNote>, IServiceImportantNoteRepository
    {
        public ServiceImportantNoteRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<ServiceImportantNote>> GetByServiceIdAsync(int govServiceId)
            => await _dbSet
                .Where(n => n.GovServiceId == govServiceId)
                .OrderBy(n => n.DisplayOrder)
                .ToListAsync();
    }
}
