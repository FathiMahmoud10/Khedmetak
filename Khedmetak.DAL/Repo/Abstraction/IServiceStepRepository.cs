using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.shared;

namespace Khedmetak.DAL.Repositories.Interfaces
{
    public interface IServiceStepRepository : IGenericRepository<ServiceSteps>
    {
        Task<IEnumerable<ServiceSteps>> GetByServiceIdAsync(int govServiceId);
    }
}
