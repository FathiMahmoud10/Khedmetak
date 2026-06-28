using Khedmetak.AI.Agents.Abstraction;
using Khedmetak.AI.Agents.Implementaion;
using Khedmetak.AI.DTOs.UserAIChatDataDto;
using Khedmetak.AI.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace Khedmetak.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatOrchestrator _chatOrchestrator;
        private readonly IChatSessionService _chatSessionService;
        private readonly IServiceIntentAgent _Intent;

        public ChatController(
            IChatOrchestrator chatOrchestrator,
            IChatSessionService chatSessionService,
            IServiceIntentAgent intent)
        {
            _chatOrchestrator = chatOrchestrator;
            _chatSessionService = chatSessionService;
            _Intent = intent;
            
        }

        [HttpPost]
        public async Task<IActionResult> Chat(
            [FromBody] UserMessageDTO request)
        {
            var session =
                await _chatSessionService.GetSessionLast15Messages(
                    request.SessionGuidId);

            if (session == null)
            {
                return NotFound("Session not found.");
            }

            var answer =
                await _chatOrchestrator.AskAsync(
                    request.Message,
                    session);

            return Ok(answer);
        }

        [HttpPost("intent")]
        public async Task<IActionResult> ServiceIntent(
           [FromBody] string request)
        {
            //var session =
            //    await _chatSessionService.GetSessionLast15Messages(
            //        request.SessionGuidId);

            //if (session == null)
            //{
            //    return NotFound("Session not found.");
            //}

            var answer =
                await _Intent.DetectIntentAsync(request);

            return Ok(answer);
        }
    }
}