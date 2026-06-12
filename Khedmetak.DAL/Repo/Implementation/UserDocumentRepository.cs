// Khedmetak.DAL/Repositories/UserDocumentRepository.cs
using Khedmetak.Core.Data;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.Abstraction;
using Khedmetak.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Khedmetak.DAL.Repositories
{
    public class UserDocumentRepository : GenericRepository<UserDocument>, IUserDocumentRepository
    {
        public UserDocumentRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<UserDocument>> GetByUserIdAsync(int userId)
            => await _dbSet
                .Where(d => d.UserId == userId)
                .ToListAsync();
    }
}