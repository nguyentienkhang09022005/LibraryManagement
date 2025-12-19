using LibraryManagement.Dto.Request;
using LibraryManagement.Service.InterFace;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AiController : ControllerBase
    {
        private readonly IChatWithAIService _chatWithAIService;

        public AiController(IChatWithAIService chatWithAIService)
        {
            _chatWithAIService = chatWithAIService;
        }

        [Authorize]
        [HttpPost("generate-message")]
        public async Task<IActionResult> SendMessageForAI([FromBody] ChatRequest chatRequest)
        {
            var result = await _chatWithAIService.SendMessageForAI(chatRequest);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpGet("get-history")]
        public async Task<IActionResult> GetHistoryChatWithAI([FromQuery] string idReader)
        {
            var result = await _chatWithAIService.GetChatHistoryAsync(idReader);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpDelete("delete-history")]
        public async Task<IActionResult> DeleteHistoryChatWithAI([FromQuery] string idReader)
        {
            var result = await _chatWithAIService.DeleteChatHistoryAsync(idReader);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(result.StatusCode, result);
        }
    }
}
