using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Khedmetak.DAL.Repo.shared
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
        Task<T?> GetByIdAsync(int id);
        Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
