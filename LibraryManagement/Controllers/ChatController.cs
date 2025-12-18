using LibraryManagement.Dto.Request;
using LibraryManagement.Models;
using LibraryManagement.Repository.IRepository;
using LibraryManagement.Service.InterFace;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
                var idManager = await _readerService.GetManagerIdAsync();

                if (idManager == null)
                {
                    return BadRequest("Hiện không có Manager nào trực tuyến để nhận tin nhắn.");
                }

                finalReceiverId = idManager;
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

            if (string.IsNullOrEmpty(userId))
                return NotFound("Không tìm thấy thông tin người dùng");

            if (role == "Reader")
            {
                var managerId = await _readerService.GetManagerIdAsync();
                if (string.IsNullOrEmpty(managerId))
                {
                    return BadRequest("Hệ thống hiện chưa có Manager để lấy lịch sử.");
                }

                var history = await _chatService.GetChatHistoryAsync(userId, managerId);
                return Ok(history);
            }

            if (role == "Manager")
            {
                if (string.IsNullOrEmpty(receiveUserId))
                {
                    return BadRequest("Manager cần truyền receiveUserId (ID của Reader)");
                }

                var history = await _chatService.GetChatHistoryAsync(userId, receiveUserId);
                return Ok(history);
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