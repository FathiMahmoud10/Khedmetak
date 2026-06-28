using Khedmetak.AI.DTOs.ChatMessagesDTO;
using Khedmetak.AI.Services.Abstraction;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.Abstraction.UnitOfWork;
using Khedmetak.DAL.Repo.shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.Services.Implementation
{
    public class ChatMessageService: IChatMessageService
    {
        private readonly IGenericRepository<ChatMessage> msgRepo;
        private readonly IGenericRepository<ChatSession> sessionRepo;

        private readonly IUnitOfWork unitOfWork;
        public ChatMessageService(IGenericRepository<ChatMessage> repo, IGenericRepository<ChatSession> sessionRepo,IUnitOfWork unitOfWork)
        {
            this.msgRepo = repo;
            this.unitOfWork = unitOfWork;
            this.sessionRepo = sessionRepo;
        }
        public async Task<bool> AddUserMessageAndResponseAsync(AddMsgAndReplyTOSessionDTO msgAndReply)
        {
            try
            {
                if (msgAndReply == null || msgAndReply.UserMessage == null || msgAndReply.AIResponse == null)
                    return false;

                var now = DateTime.UtcNow;

                var session = await sessionRepo.FindOneAsync(s => s.SessionGuid == msgAndReply.SessionGuidId);
                if (session!=null)
                {
                    var userMsg = new ChatMessage
                    {
                        ChatSessionId = session.Id,
                        Content = msgAndReply.UserMessage,
                        Role = "user",
                        StartedAt = now,
                        SentAt = now
                    };

                    var responseMsg = new ChatMessage
                    {
                        ChatSessionId = session.Id,
                        Content = msgAndReply.AIResponse,
                        Role = "assistant",
                        StartedAt = now,
                        SentAt = now
                    };

                    msgRepo.Add(userMsg);
                    msgRepo.Add(responseMsg);

                    await unitOfWork.SaveChangesAsync();

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
