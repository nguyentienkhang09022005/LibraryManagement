using LibraryManagement.Dto.Request;
using LibraryManagement.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        [HttpGet("list_reader")]

        public async Task<IActionResult> gettAllReader()
        {
            try
            {
                return Ok(await _readerService.getAllReaderAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Endpoint thêm độc giả
        [HttpPost("add_reader")]
        public async Task<IActionResult> addNewReader([FromForm] ReaderCreationRequest request)
        {
            var result = await _readerService.addReaderAsync(request);
            if (result.Success)
                return Created("", result);
            return BadRequest(result);
        }

        // Endpont sửa độc giả
        [HttpPatch("update_reader/{idReader}")]
        public async Task<IActionResult> updateReader([FromForm] ReaderUpdateRequest request, string idReader)
        {
            var result = await _readerService.updateReaderAsync(request, idReader);
            if (result.Success)
                return Ok(result);
            return NotFound(result);
        }

        // Endpoint xóa độc giả
        [HttpDelete("delete_reader/{idReader}")]
        public async Task<IActionResult> deleteReader(string idReader)
        {
            var result = await _readerService.deleteReaderAsync(idReader);
            if (result.Success)
                return Ok(result);
            return NotFound(result);
        }

        [HttpGet("find_readerby{username}")]
        public async Task<IActionResult> findReader(string username)
        {
            try
            {
                var result = await _readerService.findReaderAsync(username);
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpGet("getReaderBy{idreader}")]
        public async Task<IActionResult> getReaderById(string readerid) {
            try
            {
                var result = await _readerService.findReaderInputAsync(readerid);
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
