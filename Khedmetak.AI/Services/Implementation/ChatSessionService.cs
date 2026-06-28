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
        private readonly IChatSessionRepository sessionRepo;
        private readonly IUserRepository userRepo;
        private readonly IUnitOfWork unitOfWork;

        public ChatSessionService(IChatSessionRepository repo,IUnitOfWork unitOfWork,IUserRepository userRepo)
        {
            sessionRepo = repo;
            this.unitOfWork = unitOfWork;
            this.userRepo = userRepo;
        }

        //               =========== Add New Session ===========
        public async Task<Guid> AddNewSession(NewSessionDTO newSessionDTO)
        {
            //var systemPrompt = new ChatMessage() { Role = "system",
            //    Content= """
            //    You are an Egyptian Government Services Assistant.

            //    Always answer in Egyptian Arabic.

            //    Formatting rules:
            //    - Use Markdown.
            //    - Use headings (##).
            //    - Use bullet lists (-).
            //    - Use numbered lists (1. 2. 3.).
            //    - Never output JSON.
            //    - Never output escaped characters such as \n or \r\n.
            //    - Keep answers concise and structured.

            //    Response Template:

            //    # {Service Name}

            //    ## 📋 Required Documents
            //    - Document 1
            //    - Document 2

            //    ## 📝 Steps
            //    1. Step 1
            //    2. Step 2

            //    ## 💰 Fees
            //    - Fee information
            //    - If unavailable, write: "غير متوفر حالياً"

            //    ## ⏳ Processing Time
            //    - Processing time
            //    - If unavailable, write: "غير متوفر حالياً"


            //    """
            //};
            User? user = await userRepo.GetUserAsync(u => u.Email == newSessionDTO.UserEmail);
            //var userId = user.Id;
            //int? userId = user;

            var session = new ChatSession()
            {
                StartedAt = newSessionDTO.CreatedAt,
                UserId = user.Id

            };
            //session.ChatMessages.Add(systemPrompt);

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

        public async Task<ChatSessionDTO?> GetSessionLast15Messages(Guid sessionGuidId)
        {
            var chatSession = await sessionRepo.FindOneAsync(s => s.SessionGuid == sessionGuidId);

            //  ------------ if session doesn't Exist
            if (chatSession == null)
                return null;
            var chatSessionLastMessages = await sessionRepo.GetLastMessagesAsync(sessionGuidId, 15);
            // --------------  if session Exist but not has messages yet
            // -------------- then return new empty list of ChatSessionMessageDTO
            if (chatSessionLastMessages == null || chatSessionLastMessages.Count ==0)
                return new ChatSessionDTO()
                {
                    SessionGuidId = sessionGuidId,
                    ChatSession_ChatHistory = new List<ChatSessionMessageDTO>()
                };


            // --------------- if session exist and has messages
            // ----- then return new ChatSessionMessageDTO that has session ID and all Messages of it
            var chatHistory = new List<ChatSessionMessageDTO>();

            foreach (var message in chatSession.ChatMessages)
            {
                chatHistory.Add(new ChatSessionMessageDTO
                {
                    MessageId = message.Id,
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

        public async Task<List<UserSessionsDTO>?> GetAllSessionOfUserAsync(string userMail)
        {
            User? user = await userRepo.GetUserAsync(u => u.Email == userMail);
            
            if (user == null) return null;

            int? userId = user.Id;

            var userSessions = await sessionRepo.FindAsync(s=> s.UserId == userId);
            List<UserSessionsDTO> userSessionsDTO = new List<UserSessionsDTO>();
            foreach (var session in userSessions)
            {
                UserSessionsDTO UserSessionDTO = new UserSessionsDTO()
                {
                    GuidId = session.SessionGuid,
                    StartedAt = session.StartedAt
                };
                userSessionsDTO.Add(UserSessionDTO);
            }
            return userSessionsDTO;
        }



    }
}
