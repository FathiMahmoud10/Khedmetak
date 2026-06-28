using Khedmetak.AI.DTOs.ChatSessionDTO;
using Khedmetak.AI.DTOs.UserAIChatDataDto;
using Khedmetak.AI.Services.Abstraction;
using Khedmetak.BLL.ApiResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Khedmetak.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class SessionController : ControllerBase
    {

        private readonly IChatSessionService sessionService;

        public SessionController(IChatSessionService sessionService)
        {

            this.sessionService = sessionService;
        }

        //================= Start new session =================

        [HttpPost("newSession")]
        public async Task<IActionResult> NewSession(NewSessionDTO dto)
        {
            var sessionId = await sessionService.AddNewSession(dto);
            return Ok(ApiResponse<Guid>.Ok(sessionId));
        }

        //             ========================= Get Session Messages by Session Id ==========================

        [HttpGet("SessionMsgs/{sessionGuidId}")]
        public async Task<IActionResult> SessionMsgs(Guid sessionGuidId)
        {
            var msgs = await sessionService.GetSessionAllMessages(sessionGuidId);

            if (msgs == null)
            {
                return Ok(ApiResponse<List<ChatSessionMessageDTO>>.Fail("Session not found"));
            }

            return Ok(
                ApiResponse<List<ChatSessionMessageDTO>>.Ok(
                    msgs.ChatSession_ChatHistory ?? new List<ChatSessionMessageDTO>()
                )
            );
        }

        [HttpGet("UserSessions/{userMail}")]
        public async Task<IActionResult> UserSessions(string userMail)
        {
            var userSessions = await sessionService.GetAllSessionOfUserAsync(userMail);
            if (userSessions == null) return NotFound();
            return Ok(ApiResponse<List<UserSessionSummaryDTO>>.Ok(userSessions));
        }


    }
}
