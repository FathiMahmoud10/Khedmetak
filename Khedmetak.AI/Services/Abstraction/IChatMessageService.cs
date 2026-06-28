using Khedmetak.AI.DTOs.ChatMessagesDTO;
using Khedmetak.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.Services.Abstraction
{
    public interface IChatMessageService
    {
        public Task<bool> AddUserMessageAndResponseAsync(AddMsgAndReplyTOSessionDTO msgAndReply);
    }
}
