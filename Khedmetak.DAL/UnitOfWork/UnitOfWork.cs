using Khedmetak.Core.Data;
using Khedmetak.DAL.Repositories;
using Khedmetak.DAL.Repositories.Interfaces;

namespace Khedmetak.DAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        // Lazy-initialized repositories
        private IGovServiceRepository? _govServices;
        private ICategoryRepository? _categories;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IGovServiceRepository GovServices
            => _govServices ??= new GovServiceRepository(_context);

        public ICategoryRepository Categories
            => _categories ??= new CategoryRepository(_context);

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public void Dispose()
            => _context.Dispose();
    }
}
