
using Khedmetak.AI.DTOs;
using Khedmetak.AI.DTOs.ChatSessionDTO;
using Khedmetak.AI.Services.Abstraction;
using Khedmetak.BLL.ApiResponse;
using Khedmetak.BLL.DTOS.Categorys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Khedmetak.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIController : ControllerBase
    {
        private readonly IAIChatService aiService;
        private readonly IChatSessionService sessionService;
        private readonly IChatMessageService messageService;

        public AIController(IAIChatService aiService, IChatSessionService sessionService, IChatMessageService messageService)
        {
            this.aiService = aiService;
            this.sessionService = sessionService;
            this.messageService = messageService;
        }


        [HttpPost("newSession")]
        public async Task<IActionResult> NewSession()
        {
           var sessionId = await sessionService.AddNewSession();
            return Ok(ApiResponse<int>.Ok(sessionId));
        }

        
        [HttpPost("SessionMsgs")]
        public async Task<IActionResult> SessionMsgs([FromBody] int sessionId)
        {
            var msgs = await sessionService.GetSessionAllMessages(sessionId);

            if (msgs == null)
            {
                return Ok(ApiResponse<List<ChatSessionMessageDTO>>.Fail("Session not found"));
            }

            return Ok(
                ApiResponse<List<ChatSessionMessageDTO>>.Ok(
                    msgs.SessionChatHistory ?? new List<ChatSessionMessageDTO>()
                )
            );
        }

        //[HttpPost("chat1")]
        //public async Task<IActionResult> Chat1([FromBody] string message)
        //{
        //    if (message == null || string.IsNullOrWhiteSpace(message))
        //    {
        //        return BadRequest("Message is required.");
        //    }

            
        //    var aiResponse = await aiService.AskAsync(message);

        //    return Ok(aiResponse);
        //}


        [HttpPost("chat2")]
        public async Task<IActionResult> Chat2([FromBody] UserMessageDTO userMessageDTO)
        {
            if (userMessageDTO == null || string.IsNullOrWhiteSpace(userMessageDTO.Message))
            {
                return BadRequest(ApiResponse<string>.Fail("Message is required."));
            }

            if (userMessageDTO.sessionId == null || userMessageDTO.sessionId == -1)
            {
                return BadRequest(ApiResponse<string>.Fail("Not Available to send message without sessionId"));

            }
           
                
               var sessionDTO = await sessionService.GetSessionAllMessages(userMessageDTO.sessionId);

                if (sessionDTO == null)
                {
                    return NotFound(ApiResponse<string>.Fail("Invalid SessionId"));
                }
            

            var aiResponse = await aiService.AskAsync(userMessageDTO.Message, sessionDTO);
            ChatResponseDTO response = new ChatResponseDTO()
            {
                Message = aiResponse,
                SessionId = userMessageDTO.sessionId
            };
            AddMsgAndReplyTOSessionDTO msgAndReply = new()
            {
                UserMessage = userMessageDTO.Message,
                AIResponse = aiResponse
            };
            await messageService.AddMessageAsync(userMessageDTO.sessionId,msgAndReply);
            return Ok(ApiResponse<ChatResponseDTO>.Ok(response));
        }
    }
}
