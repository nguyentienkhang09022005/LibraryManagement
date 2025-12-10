using LibraryManagement.Dto.Request;
using LibraryManagement.Repository.InterFace;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookReceiptController : ControllerBase
    {
        private readonly IBookReceiptService _bookReceiptService;

        public BookReceiptController(IBookReceiptService bookReceiptService)
        {
            _bookReceiptService = bookReceiptService;
        }

        // Endpoint thêm phiếu nhập sách
        [Authorize]
        [HttpPost("add-bookreceipt")]
        public async Task<IActionResult> addBookReceipt([FromBody] BookReceiptRequest request)
        {
            var result = await _bookReceiptService.AddBookReceiptAsync(request);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        // Endpoint xóa phiếu nhập sách
        [Authorize]
        [HttpDelete("delete-bookreceipt")]
        public async Task<IActionResult> deleteBookReceipt([FromQuery] Guid idBookReceipt)
        {
            var result = await _bookReceiptService.DeleteBookReceiptAsync(idBookReceipt);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpGet("list-receipts")]
        public async Task<IActionResult> getAllReceipt([FromQuery] string token)
        {
            var result = await _bookReceiptService.GetAllReceiptHistory(token);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }
    }
}
