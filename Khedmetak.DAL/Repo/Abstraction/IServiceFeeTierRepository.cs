using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.shared;

namespace Khedmetak.DAL.Repositories.Interfaces
{
    public interface IServiceFeeTierRepository : IGenericRepository<ServiceFeeTier>
    {
        Task<IEnumerable<ServiceFeeTier>> GetByServiceIdAsync(int govServiceId);
    }
}
