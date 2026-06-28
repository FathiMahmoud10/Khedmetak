using Khedmetak.AI.DTOs;
using Khedmetak.AI.DTOs.ChatSessionDTO;
using Khedmetak.AI.Services.Abstraction;
using Khedmetak.DAL.Entities;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<User> userManager;

        public ChatSessionService(IGenericRepository<ChatSession> repo, IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            sessionRepo = repo;
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
        }

        //               =========== Add New Session ===========
        public async Task<Guid> AddNewSession(NewSessionDTO newSessionDTO)
        {
            if (newSessionDTO == null)
            {
                newSessionDTO = new NewSessionDTO { CreatedAt = DateTime.UtcNow };
            }

            var systemPrompt = new ChatMessage()
            {
                Role = "system",
                Content = """
               You are an Egyptian Government Services Assistant.

               Always answer in Egyptian Arabic.

               Formatting rules:
               - Use Markdown.
               - Use headings (##).
               - Use bullet lists (-).
               - Use numbered lists (1. 2. 3.).
               - Never output JSON.
               - Never output escaped characters such as \n or \r\n.
               - Keep answers concise and structured.

               Response Template:

               # {Service Name}

               ## 📋 Required Documents
               - Document 1
               - Document 2

               ## 📝 Steps
               1. Step 1
               2. Step 2

               ## 💰 Fees
               - Fee information
               - If unavailable, write: "غير متوفر حالياً"

               ## ⏳ Processing Time
               - Processing time
               - If unavailable, write: "غير متوفر حالياً"


               """
            };
            int? userId = null;
            if (userManager != null &&
                !string.IsNullOrWhiteSpace(newSessionDTO.UserEmail) &&
                !newSessionDTO.UserEmail.Equals("guest@moamaltak.ai", StringComparison.OrdinalIgnoreCase))
            {
                var user = await userManager.FindByEmailAsync(newSessionDTO.UserEmail);
                if (user != null)
                {
                    userId = user.Id;
                }
            }

            var session = new ChatSession()
            {
                StartedAt = newSessionDTO.CreatedAt,
                UserId = userId
            };

            if (session.ChatMessages == null)
            {
                session.ChatMessages = new List<ChatMessage>();
            }
            session.ChatMessages.Add(systemPrompt);

            sessionRepo.Add(session);
            await unitOfWork.SaveChangesAsync();

            return session.SessionGuid;

        }
        public async Task<List<UserSessionSummaryDTO>> GetAllSessionOfUserAsync(string userMail)
        {
            return await GetUserSessionsAsync(userMail);
        }

        //      ===================  الحصول على كل الرسابل الخاصة ب chatSession معينة =================
        public async Task<ChatSessionDTO?> GetSessionAllMessages(Guid sessionGuidId)
        {
            var chatSession = await sessionRepo.FindOneAsync(s => s.SessionGuid == sessionGuidId, s => s.ChatMessages);

            //  ------------ if session doesn't Exist
            if (chatSession == null)
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
        public async Task<ChatSessionDTO?> GetSessionLast15Messages(Guid sessionGuidId)
        {
            var chatSession = await sessionRepo.FindOneAsync(
                s => s.SessionGuid == sessionGuidId,
                s => s.ChatMessages);

            if (chatSession == null)
                return null;

            var last15 = chatSession.ChatMessages?
                .Where(m => m.Role != "system")
                .OrderByDescending(m => m.SentAt)
                .Take(15)
                .OrderBy(m => m.SentAt)
                .ToList() ?? new List<ChatMessage>();

            var chatHistory = last15.Select(m => new ChatSessionMessageDTO
            {
                MessageId = m.Id,
                Content = m.Content,
                Role = m.Role,
            }).ToList();

            return new ChatSessionDTO
            {
                SessionGuidId = sessionGuidId,
                ChatSession_ChatHistory = chatHistory
            };
        }

        public async Task<List<UserSessionSummaryDTO>> GetUserSessionsAsync(string userEmail)
        {
            if (string.IsNullOrWhiteSpace(userEmail) || userEmail.Equals("guest@moamaltak.ai", StringComparison.OrdinalIgnoreCase))
            {
                return new List<UserSessionSummaryDTO>();
            }

            var user = await userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return new List<UserSessionSummaryDTO>();
            }

            var sessions = await sessionRepo.FindAllByAsync(s => s.UserId == user.Id, s => s.ChatMessages);

            return sessions
                .OrderByDescending(s => s.StartedAt)
                .Select(s => {
                    var firstUserMsg = s.ChatMessages?
                        .Where(m => m.Role == "user")
                        .OrderBy(m => m.SentAt)
                        .FirstOrDefault();

                    return new UserSessionSummaryDTO
                    {
                        Id = s.Id,
                        SessionGuidId = s.SessionGuid,
                        StartedAt = s.StartedAt,
                        EndedAt = s.EndedAt,
                        Preview = firstUserMsg?.Content ?? string.Empty,
                        MessageCount = s.ChatMessages?.Count(m => m.Role == "user") ?? 0
                    };
                }).ToList();
        }
    }
}