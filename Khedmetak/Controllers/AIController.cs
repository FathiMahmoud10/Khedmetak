
using Khedmetak.AI.DTOs;
using Khedmetak.AI.DTOs.ChatSessionDTO;
using Khedmetak.AI.Services.Abstraction;
using Khedmetak.BLL.ApiResponse;
using Khedmetak.BLL.DTOS.Categorys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Khedmetak.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
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

        //                       ============= Just for insure that model is work ===============

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


        //           =============== To Send new message from user to AI model and return the Response from AI ===================

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] UserMessageDTO userMessageDTO)
        {
            //          --------- if user not write anything in the message 
            if (userMessageDTO == null || string.IsNullOrWhiteSpace(userMessageDTO.Message))
            {
                return BadRequest(ApiResponse<string>.Fail("Message is required."));
            }

            //          ----------- if the sessionGuidId is null 

            if (userMessageDTO.SessionGuidId == null )
            {
                return BadRequest(ApiResponse<string>.Fail("Not Available to send message without sessionId"));

            }
            
            //      ------------ (1) get all messages of session by session Guid id ----------
            var sessionDTO = await sessionService.GetSessionAllMessages(userMessageDTO.SessionGuidId);

            //              --------- if session is not exist ----> return not found

            if (sessionDTO == null)
            {
                return NotFound(ApiResponse<string>.Fail("Invalid SessionId"));
            }
            //              ---------  session exist and message exist 
            //          ----------- (2) then send the message to AI and wait for response

            var aiResponse = await aiService.AskAsync(userMessageDTO.Message, sessionDTO);

                //  ------- (3) save user message and AI response to database
            AddMsgAndReplyTOSessionDTO msgAndReply = new()
            {
                SessionGuidId = userMessageDTO.SessionGuidId,
                UserMessage = userMessageDTO.Message,
                AIResponse = aiResponse
            };
            await messageService.AddUserMessageAndResponseAsync(msgAndReply);

            // ----------- (4) send AI response to the session of user
            ChatResponseDTO response = new ChatResponseDTO()
            {
                Message = aiResponse,
                SessionGuidId = userMessageDTO.SessionGuidId
            };
            return Ok(ApiResponse<ChatResponseDTO>.Ok(response));
        }
    }
}
