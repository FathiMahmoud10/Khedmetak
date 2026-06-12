using Khedmetak.DAL.Repositories.Interfaces;

namespace Khedmetak.DAL.Repo.Abstraction.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IGovServiceRepository GovServices { get; }
        ICategoryRepository Categories { get; }
        IUserDocumentRepository UserDocuments { get; }   

        Task<int> SaveChangesAsync();
    }
}
