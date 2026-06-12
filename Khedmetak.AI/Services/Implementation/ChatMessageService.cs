using Khedmetak.AI.DTOs;
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
        private readonly IUnitOfWork unitOfWork;
        public ChatMessageService(IGenericRepository<ChatMessage> repo, IUnitOfWork unitOfWork)
        {
            this.msgRepo = repo;
            this.unitOfWork = unitOfWork;
        }
        //       ========= Add and Save "User and Response" Messages to database ============
        public async Task<bool> AddMessageAsync(int sessionId,AddMsgAndReplyTOSessionDTO msgAndReply)
        {
            try
            {
                if (msgAndReply == null || msgAndReply.UserMessage == null || msgAndReply.AIResponse == null)
                    return false;

                var now = DateTime.UtcNow;

                var userMsg = new ChatMessage
                {
                    ChatSessionId = sessionId,
                    Content = msgAndReply.UserMessage,
                    Role = "user",
                    StartedAt = now,
                    SentAt = now
                };

                var responseMsg = new ChatMessage
                {
                    ChatSessionId = sessionId,
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
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
