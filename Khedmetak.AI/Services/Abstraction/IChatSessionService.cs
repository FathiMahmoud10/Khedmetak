using Khedmetak.AI.DTOs;
using Khedmetak.AI.DTOs.ChatSessionDTO;
using Khedmetak.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.Services.Abstraction
{
    public interface IChatSessionService
    {

        public Task<Guid> AddNewSession(NewSessionDTO newSessionDTO);
     
        public Task<ChatSessionDTO?> GetSessionAllMessages(Guid sessionGuidId);
        public Task<ChatSessionDTO?> GetSessionLast15Messages(Guid sessionGuidId);



        //public Task<ChatSession?> GetSessionById(int id);

        //Task AddUserAndAiMessagesAsync(int sessionId, string userMessage, string aiResponse);

        //Task<bool> DeleteSessionAsync(int sessionId);

        //Task RenameSessionAsync(int sessionId, string title);


    }
}
