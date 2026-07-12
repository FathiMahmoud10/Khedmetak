using Khedmetak.AI.DTOs.ChatMessagesDTO;
using Khedmetak.AI.DTOs.ChatSessionDTO;
using Khedmetak.AI.DTOs.UserAIChatDataDto;
using Khedmetak.AI.Orchestrators;
using Khedmetak.AI.RAG;
using Khedmetak.AI.Services.Abstraction;
using Khedmetak.AI.Services.Implementation;
using Khedmetak.BLL.ApiResponse;
using Khedmetak.BLL.DTOS.Categorys;
using Khedmetak.BLL.Services.Abstraction;
using Khedmetak.BLL.Services.Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qdrant.Client.Grpc;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Khedmetak.DAL.Entities;

namespace Khedmetak.Controllers
{
    [Route("api/[controller]")]
    //[Authorize]
    [ApiController]
    [AllowAnonymous]
    public class AIController : ControllerBase
    {
        private readonly IChatOrchestrator chatOrchestrator;
        //private readonly IAIChatService _aIChatService;
        private readonly IChatSessionService sessionService;
        private readonly IChatMessageService messageService;
        private readonly IDocumentService documentService;
        private readonly UserManager<User> userManager;

        public AIController(IChatOrchestrator Orchestrator, IDocumentService _documentService, IChatSessionService sessionService, IChatMessageService messageService, UserManager<User> _userManager)
        {
            this.sessionService = sessionService;
            this.messageService = messageService;
            this.chatOrchestrator = Orchestrator;
            this.documentService = _documentService;
            this.userManager = _userManager;
            //_aIChatService = aIChatService;
        }

        //                       ============= Just for insure that model is work ===============

        //[HttpPost("chat1")]
        //public async Task<IActionResult> Chat1([FromBody] string message)
        //{
        //    if (message == null || string.IsNullOrWhiteSpace(message))
        //    {
        //        return BadRequest("Message is required.");
        //    }


        //    var aiResponse = await _aIChatService.AskAsync(message);

        //    return Ok(aiResponse);
        //}


        //           =============== To Send new message from user to AI model and return the Response from AI ===================

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] UserMessageDTO userMessageDTO)
        {
            //          --------- if user not write anything in the message 
            if (userMessageDTO == null || string.IsNullOrWhiteSpace(userMessageDTO.Message))
            {
                return BadRequest("Message is required.");
            }

            //          ----------- if the sessionGuidId is null 

            if (userMessageDTO.SessionGuidId == null)
            {
                return BadRequest("Not Available to send message without sessionId");

            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim != null)
            {
                var user = await userManager.FindByIdAsync(userIdClaim);
                if (user != null)
                {
                    if (!user.HasPaidForChat && user.ChatMessagesCount >= 5)
                    {
                        return StatusCode(403, new { message = "لقد استنفدت الحد الأقصى للرسائل المجانية (5 رسائل). يرجى الدفع للاستمرار." });
                    }

                    user.ChatMessagesCount++;
                    await userManager.UpdateAsync(user);
                }
            }

            //      ------------ (1) get all messages of session by session Guid id ----------
            var sessionDTO = await sessionService.GetSessionAllMessages(userMessageDTO.SessionGuidId);

            //              --------- if session is not exist ----> return not found

            if (sessionDTO == null)
            {
                return NotFound("Invalid SessionId");
            }
            //              ---------  session exist and message exist 
            //          ----------- (2) then send the message to AI and wait for response

            var aiResponse = await chatOrchestrator.AskAsync(userMessageDTO.Message, sessionDTO);

            //  ------- (3) save user message and AI response to database
            AddMsgAndReplyTOSessionDTO msgAndReply = new()
            {
                SessionGuidId = userMessageDTO.SessionGuidId,
                UserMessage = userMessageDTO.Message,
                AIResponse = aiResponse.response
            };
            await messageService.AddUserMessageAndResponseAsync(msgAndReply);

            // ----------- (4) send AI response to the session of user
            //ChatResponseDTO response = new ChatResponseDTO()
            //{
            //    Message = aiResponse,
            //    SessionGuidId = userMessageDTO.SessionGuidId
            //};
            return Ok(aiResponse);
        }

        [HttpPost("upload-documents")]
        public async Task<IActionResult> UploadDocuments(
    [FromForm] List<IFormFile> files,
    [FromForm] int? chatSessionId)
        {
            // 1. جيب UserId من التوكن
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim);

            if (files == null || files.Count == 0)
                return BadRequest(ApiResponse<string>.Fail("No files uploaded."));

            var savedDocs = await documentService.SaveUserDocumentsWithDetailsAsync(files, userId, chatSessionId ?? 0);

            if (savedDocs == null || savedDocs.Count == 0)
                return StatusCode(500, ApiResponse<string>.Fail("Failed to save documents."));

            // بناء الـ URL الكامل لكل ملف
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var results = savedDocs.Select(doc => new
            {
                doc.Id,
                doc.FileName,
                doc.FilePath,
                doc.FileType,
                doc.UploadedAt,
                FileUrl = $"{baseUrl}{doc.FilePath}"
            }).ToList();

            return Ok(ApiResponse<object>.Ok(results, "تم رفع الملفات بنجاح"));
        }

        

      
       



    }
}
