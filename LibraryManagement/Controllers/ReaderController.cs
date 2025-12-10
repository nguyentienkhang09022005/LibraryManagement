using LibraryManagement.Dto.Request;
using LibraryManagement.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [Route("api/reader/[controller]")]
    [ApiController]
    public class ReaderController : ControllerBase
    {
        private IReaderService _readerService;

        public ReaderController(IReaderService readerService)
        {
            _readerService = readerService;
        }

        [Authorize]
        [HttpGet("list-reader")]
        public async Task<IActionResult> gettAllReader()
        {
            var result = await _readerService.getAllReaderAsync();
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpPost("add-reader")]
        public async Task<IActionResult> addNewReader([FromForm] ReaderCreationRequest request)
        {
            var result = await _readerService.addReaderAsync(request);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpPatch("update-reader")]
        public async Task<IActionResult> updateReader([FromForm] ReaderUpdateRequest request, [FromQuery] string idReader)
        {
            var result = await _readerService.updateReaderAsync(request, idReader);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpDelete("delete-reader")]
        public async Task<IActionResult> deleteReader([FromQuery] string idReader)
        {
            var result = await _readerService.deleteReaderAsync(idReader);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpGet("find-reader-by-username")]
        public async Task<IActionResult> findReader([FromQuery] string username)
        {
            var result = await _readerService.findReaderAsync(username);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpGet("get-reader-by-id")]
        public async Task<IActionResult> getReaderById([FromQuery] string idReader) 
        {
            var result = await _readerService.findReaderInputAsync(idReader);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }
    }
}
