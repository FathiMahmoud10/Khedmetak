using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.shared;

namespace Khedmetak.DAL.Repositories.Interfaces
{
    public interface IRequiredDocumentRepository : IGenericRepository<RequiredDocument>
    {
        Task<IEnumerable<RequiredDocument>> GetByServiceIdAsync(int govServiceId);
    }
}
