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
        //private readonly IGenericRepository<User> userRepo;
        private readonly IUnitOfWork unitOfWork;

        public ChatSessionService(IGenericRepository<ChatSession> repo,IUnitOfWork unitOfWork)//,IGenericRepository<User> userRepo)
        {
            sessionRepo = repo;
            this.unitOfWork = unitOfWork;
            //this.userRepo = userRepo;
        }

        //               =========== Add New Session ===========
        public async Task<Guid> AddNewSession(NewSessionDTO newSessionDTO)
        {
           var systemPrompt = new ChatMessage() { Role = "system", Content="You are an Egyptian Government assistant that help Citizen with their government services, speak in Egyptian Arabic. don't answer to another topic." };
            //User ? user = await userRepo.FindOneAsync(u => u.Email == newSessionDTO.UserEmail);
            //var userId = user.Id;
            int? userId = null;

            var session = new ChatSession()
            {
                StartedAt = newSessionDTO.CreatedAt,
                UserId = userId

            };
            session.ChatMessages.Add(systemPrompt);

            sessionRepo.Add(session); 
            await unitOfWork.SaveChangesAsync(); 

            return session.SessionGuid;

        }


        //      ===================  الحصول على كل الرسابل الخاصة ب chatSession معينة =================
        public async Task<ChatSessionDTO?> GetSessionAllMessages(Guid sessionGuidId)
        {
            var chatSession = await sessionRepo.FindOneAsync(s => s.SessionGuid == sessionGuidId, s => s.ChatMessages);

            //  ------------ if session doesn't Exist
            if (chatSession == null )
                return null;

            // --------------  if session Exist but not has messages yet
            // -------------- then return new empty list of ChatSessionMessageDTO
            if (chatSession.ChatMessages == null)
                return new ChatSessionDTO()
                {
                    SessionGuidId = sessionGuidId,
                    ChatSession_ChatHistory = new List<ChatSessionMessageDTO>()
                };

            //var chatsessionWithMessages = await()
            // --------------- if session exist and has messages
            // ----- then return new ChatSessionMessageDTO that has session ID and all Messages of it
            var chatHistory = new List<ChatSessionMessageDTO>();

            foreach (var message in chatSession.ChatMessages)
            {
                chatHistory.Add(new ChatSessionMessageDTO
                {   MessageId= message.Id,
                    Content = message.Content,
                    Role = message.Role,
                });
            }

            return new ChatSessionDTO()
            {
                SessionGuidId = sessionGuidId,
                ChatSession_ChatHistory = chatHistory
            };
        }

        


    }
}
