using Khedmetak.DAL.Repositories.Interfaces;

namespace Khedmetak.DAL.Repo.Abstraction.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IGovServiceRepository GovServices { get; }
        ICategoryRepository Categories { get; }

        Task<int> SaveChangesAsync();
    }
}
