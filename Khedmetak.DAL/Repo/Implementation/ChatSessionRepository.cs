using Khedmetak.Core.Data;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.Abstraction;
using Khedmetak.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.DAL.Repo.Implementation
{
    public class ChatSessionRepository : GenericRepository<ChatSession>, IChatSessionRepository
    {
        private readonly AppDbContext _context;

        public ChatSessionRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<ChatMessage>?> GetLastMessagesAsync(
            Guid sessionGuid,
            int count)
        {
            return await _context.ChatMessages
                .Where(m => m.ChatSession.SessionGuid == sessionGuid)
                .OrderByDescending(m => m.StartedAt)
                .Take(count)
                .OrderBy(m => m.StartedAt)
                .ToListAsync();
        }
    }
}
