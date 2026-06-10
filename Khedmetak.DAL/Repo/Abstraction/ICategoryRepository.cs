using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.shared;

namespace Khedmetak.DAL.Repositories.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<IEnumerable<Category>> GetAllWithServicesCountAsync();
    }
}