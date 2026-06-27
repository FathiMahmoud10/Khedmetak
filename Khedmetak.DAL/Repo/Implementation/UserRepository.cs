using Khedmetak.Core.Data;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.Abstraction;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.DAL.Repo.Implementation
{
    public class UserRepository:IUserRepository
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<User> _dbSet;
        public UserRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<User>();
        }
        //public async Task<User?> GetUserAsync(Expression<Func<User, bool>> predicate)
        //{
        //    return await _context.Users
        //        .FirstOrDefaultAsync(predicate);
        //}
        public async Task<User?> GetUserAsync( Expression<Func<User, bool>> predicate,params Expression<Func<User, object>>[] includes)
        {
            IQueryable<User> query = _context.Users;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(predicate);
        }
    }
}
