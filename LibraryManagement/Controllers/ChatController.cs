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
            if (senderId == null) return BadRequest("Vui lòng đăng nhập");
            if (message == null) {
                return BadRequest("Vui lòng nhập tin nhắn");
            }

            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            string receiverId;

            if (role == "Manager")
            {
                if (string.IsNullOrEmpty(message.ReceiverId))
                    return BadRequest("Manager phải chọn người nhận");

                receiverId = message.ReceiverId;
            }
            else
            {
                receiverId = await _readerService.GetManagerIdAsync();
                if (receiverId == null)
                    return NotFound("Không tìm thấy Manager");
            }

            Message messageSent = new Message
            {
                SenderId = senderId,
                ReceiverId = message.ReceiverId,
                SentAt = message.SentAt,
                Content = message.Content,
            };
            await _chatService.SendMessageAsync(messageSent);
            return Ok(messageSent);
        }

        [Authorize]
        [HttpGet("history")]
        public async Task<IActionResult> History([FromQuery] string receiveUserId) {
            var sendUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (sendUserId == null) return NotFound("Không tìm thấy thông tin người dùng");
            return Ok(await _chatService.GetAllMessagesAsync(sendUserId, receiveUserId));

        }

        [Authorize]
        [HttpGet("get-all-user-sent-message")]
        public async Task<IActionResult> GetAllUserMessage()
        {
            var sender = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (sender == null) return NotFound("Không tìm thấy thông tin người dùng");
            return Ok(await _chatService.getAllMessageClient(sender));
        }
    }
}