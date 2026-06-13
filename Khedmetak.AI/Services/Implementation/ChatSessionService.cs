using Khedmetak.AI.DTOs;
using Khedmetak.AI.DTOs.ChatSessionDTO;
using Khedmetak.AI.Services.Abstraction;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.Abstraction;
using Khedmetak.DAL.Repo.Abstraction.UnitOfWork;
using Khedmetak.DAL.Repo.Implementation.UnitOfWork;
using Khedmetak.DAL.Repo.shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.Services.Implementation
{
    public class ChatSessionService : IChatSessionService
    {
        private readonly IGenericRepository<ChatSession> sessionRepo;
        private readonly IUnitOfWork unitOfWork;

        public ChatSessionService(IGenericRepository<ChatSession> repo,IUnitOfWork unitOfWork) {
            sessionRepo = repo;
            this.unitOfWork = unitOfWork;
        }

        //               =========== Add New Session ===========
        public async Task<int> AddNewSession(NewSessionDTO newSessionDTO)
        {
           var systemPrompt = new ChatMessage() { Role = "system", Content="You are an Egyptian Government assistant that help Citizen with their government services, speak in Egyptian Arabic" };

            var session = new ChatSession()
            {
                StartedAt = newSessionDTO.CreatedAt,
                UserId = newSessionDTO.UserId,
                
            };
            session.ChatMessages.Add(systemPrompt);

            sessionRepo.Add(session); 
            await unitOfWork.SaveChangesAsync(); 

            return session.Id;

        }


        //      ===================  الحصول على كل الرسابل الخاصة ب chatSession معينة =================
        public async Task<ChatSessionDTO?> GetSessionAllMessages(int sessionId)
        {
            var chatSession = await sessionRepo.GetByIdAsync(sessionId, x => x.ChatMessages);

            //  ------------ if session doesn't Exist
            if (chatSession == null )
                return null;

            // --------------  if session Exist but not has messages yet
            // -------------- then return new empty list of ChatSessionMessageDTO
            if (chatSession.ChatMessages == null)
                return new ChatSessionDTO()
                {
                    SessionId = sessionId,
                    ChatSession_ChatHistory = new List<ChatSessionMessageDTO>()
                };

            // --------------- if session exist and has messages
            // ----- then return new ChatSessionMessageDTO that has session ID and all Messages of it
            var chatHistory = new List<ChatSessionMessageDTO>();

            foreach (var message in chatSession.ChatMessages)
            {
                chatHistory.Add(new ChatSessionMessageDTO
                {   Id= message.Id,
                    Content = message.Content,
                    Role = message.Role,
                });
            }

            return new ChatSessionDTO()
            {
                SessionId = sessionId,
                ChatSession_ChatHistory = chatHistory
            };
        }

        


    }
}
