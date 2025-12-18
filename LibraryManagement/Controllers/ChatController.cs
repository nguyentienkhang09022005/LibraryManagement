using LibraryManagement.Dto.Request;
using LibraryManagement.Models;
using LibraryManagement.Repository.IRepository;
using LibraryManagement.Service.InterFace;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IReaderService _readerService;

        public ChatController(IChatService chatService, IReaderService readerService)
        {
            _chatService = chatService;
            _readerService = readerService;
        }

        [Authorize]
        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] MessageRequest message)
        {
            var senderId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (senderId == null) return BadRequest("Vui lòng đăng nhập");

            string finalReceiverId;

            if (role == "Manager")
            {
                if (string.IsNullOrEmpty(message.ReceiverId))
                    return BadRequest("Manager phải chọn người nhận");
                finalReceiverId = message.ReceiverId;
            }
            else 
            {
                finalReceiverId = "";
            }

            Message messageSent = new Message
            {
                SenderId = senderId,
                ReceiverId = finalReceiverId,
                SentAt = DateTime.UtcNow,
                Content = message.Content,
            };

            await _chatService.SendMessageAsync(messageSent);
            return Ok(messageSent);
        }

        [Authorize]
        [HttpGet("history-chat")]
        public async Task<IActionResult> History([FromQuery] string? receiveUserId) 
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userId == null)
                return NotFound("Không tìm thấy thông tin người dùng");

            if (role == "Reader")
            {
                return Ok(await _chatService.GetChatWithManagerAsync(userId));
            }

            if (role == "Manager")
            {
                if (string.IsNullOrEmpty(receiveUserId))
                    return BadRequest("Manager cần truyền readerId");

                return Ok(await _chatService.GetChatWithReaderAsync(receiveUserId, userId));
            }

            return Forbid();
        }

        [Authorize]
        [HttpGet("get-all-reader-sent-message")]
        public async Task<IActionResult> GetAllUserMessage()
        {
            var sender = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (sender == null) return NotFound("Không tìm thấy thông tin người dùng");
            return Ok(await _chatService.getAllMessageClient(sender));
        }
    }
}