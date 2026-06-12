using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.DAL.Repo.Abstraction
{
    public interface IUserDocumentRepository : IGenericRepository<UserDocument>
    {
        Task<IEnumerable<UserDocument>> GetByUserIdAsync(int userId);
    }
}
