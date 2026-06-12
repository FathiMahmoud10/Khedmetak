
using Khedmetak.AI.DTOs;
using Khedmetak.AI.DTOs.ChatSessionDTO;
using Khedmetak.AI.Services.Abstraction;
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

        public AIController(IAIChatService aiService, IChatSessionService sessionService)
        {
            this.aiService = aiService;
            this.sessionService = sessionService;
        }

        [HttpPost("chat1")]
        public async Task<ActionResult<string>> Chat1([FromBody] string message)
        {
            if (message == null || string.IsNullOrWhiteSpace(message))
            {
                return BadRequest("Message is required.");
            }

            ChatSessionDTO sessionDTO;

            //if (userMessageDTO.sessionId == -1)
            //{
            //    sessionDTO = await sessionService.AddNewSession();
            //}
            //else
            //{

            //    sessionDTO = await sessionService.GetSessionAllMessages(userMessageDTO.sessionId);

            //    if (sessionDTO == null)
            //    {
            //        return NotFound("Session not found.");
            //    }
            //}

            var aiResponse = await aiService.AskAsync(message);

            return Ok(aiResponse);
        }


        [HttpPost("chat2")]
        public async Task<ActionResult<string>> Chat2([FromBody] UserMessageDTO userMessageDTO)
        {
            if (userMessageDTO == null || string.IsNullOrWhiteSpace(userMessageDTO.Message))
            {
                return BadRequest("Message is required.");
            }

            ChatSessionDTO sessionDTO;

            if (userMessageDTO.sessionId == -1)
            {
                sessionDTO = await sessionService.AddNewSession();
            }
            else
            {
                
                sessionDTO = await sessionService.GetSessionAllMessages(userMessageDTO.sessionId);

                if (sessionDTO == null)
                {
                    return NotFound("Session not found.");
                }
            }

            var aiResponse = await aiService.AskAsync(userMessageDTO.Message, sessionDTO);

            return Ok(aiResponse);
        }
    }
}
