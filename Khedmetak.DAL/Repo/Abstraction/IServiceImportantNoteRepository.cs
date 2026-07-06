using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.shared;

namespace Khedmetak.DAL.Repositories.Interfaces
{
    public interface IServiceImportantNoteRepository : IGenericRepository<ServiceImportantNote>
    {
        Task<IEnumerable<ServiceImportantNote>> GetByServiceIdAsync(int govServiceId);
    }
}
