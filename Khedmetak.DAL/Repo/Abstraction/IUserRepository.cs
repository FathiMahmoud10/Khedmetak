using Khedmetak.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.DAL.Repo.Abstraction
{
    public interface IUserRepository
    {
        //public Task<User?> GetUserAsync(Expression<Func<User, bool>> predicate);
        public  Task<User?> GetUserAsync(Expression<Func<User, bool>> predicate, params Expression<Func<User, object>>[] includes);




    }
}
