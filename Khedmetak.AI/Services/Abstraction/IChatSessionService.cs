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

        public Task<ChatSessionDTO> AddNewSession();
        public Task<ChatSession?> GetSessionById(int id);
        public Task<ChatSessionDTO?> GetSessionAllMessages(int sessionId);

        //public Task<ChatMessage> AddMessageAsync(int sessionId, ChatMessageDTO message);

        //Task AddUserAndAiMessagesAsync(int sessionId, string userMessage, string aiResponse);

        //Task<bool> DeleteSessionAsync(int sessionId);

        //Task RenameSessionAsync(int sessionId, string title);


    }
}
