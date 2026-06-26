using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.shared;
using Khedmetak.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.DAL.Repo.Abstraction
{
    public interface IChatSessionRepository : IGenericRepository<ChatSession>
    {
        public Task<List<ChatMessage>?> GetLastMessagesAsync(Guid sessionGuid, int count);

       
        public Task<List<ChatSession>> GetByUserIdWithDetailsAsync(int userId);

        public Task<ChatSession?> GetBySessionGuidAsync(Guid sessionGuid);
    }
}
