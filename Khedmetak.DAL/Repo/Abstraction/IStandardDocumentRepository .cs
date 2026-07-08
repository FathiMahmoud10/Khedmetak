// IStandardDocumentRepository.cs
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.shared;

namespace Khedmetak.DAL.Repositories.Interfaces
{
    public interface IStandardDocumentRepository : IGenericRepository<StandardDocument>
    {
        Task<IEnumerable<StandardDocument>> GetAllAsync();
        Task<StandardDocument?> GetByIdWithUsagesAsync(int id);
    }
}