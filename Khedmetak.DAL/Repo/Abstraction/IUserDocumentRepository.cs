using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.shared;

namespace Khedmetak.DAL.Repositories.Interfaces
{
    public interface IUserDocumentRepository : IGenericRepository<UserDocument>
    {
        Task<IEnumerable<UserDocument>> GetByUserIdAsync(int userId);
    }
}