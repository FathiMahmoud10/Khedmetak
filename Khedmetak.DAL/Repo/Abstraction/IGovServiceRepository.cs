using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.shared;

namespace Khedmetak.DAL.Repositories.Interfaces
{
    public interface IGovServiceRepository : IGenericRepository<GovService>
    {
        Task<GovService?> GetServiceWithDetailsAsync(int id);
        Task<IEnumerable<GovService>> GetAllWithCategoryAsync();
        Task<IEnumerable<GovService>> GetByCategoryAsync(int categoryId);
    }
}