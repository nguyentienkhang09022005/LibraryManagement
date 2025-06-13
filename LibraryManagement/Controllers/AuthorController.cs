using LibraryManagement.Data;
using LibraryManagement.Dto.Request;
using LibraryManagement.Repository.InterFace;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ZstdSharp.Unsafe;

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;
        private readonly LibraryManagermentContext _context; 

        public AuthorController(IAuthorService authorService, LibraryManagermentContext context)
        {
            _authorService = authorService;
            _context = context;
        }

        // Endpoint lấy danh sách tác giả

        [HttpGet("list_author")]
        [Authorize]
        public async Task<IActionResult> gettListAuthor()
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ;
            if (string.IsNullOrEmpty(user))
            {
                return NotFound("Không tìm thấy thông tin người dùng");
            }
            try
            {
                var result = await _authorService.getListAuthor();
                if (result == null) return Unauthorized("Vui lòng đăng nhập");
                return Ok(result); 
            }
            catch
            {
                return BadRequest();
            }
        }

        // Endpoint thêm tác giả
        [HttpPost("add_author")]
        public async Task<IActionResult> addAuthor([FromForm] AuthorRequest request)
        {
            var result = await _authorService.addAuthorAsync(request);
            if (result.Success)
                return Created("", result);
            return BadRequest(result);
        }

        // Endpoint sửa tác giả
        [HttpPatch("update_author/{idAuthor}")]
        public async Task<IActionResult> updateAuthor([FromForm] AuthorRequest request, Guid idAuthor)
        {
            var result = await _authorService.updateAuthorAsync(request, idAuthor);
            if (result.Success)
                return Ok(result);
            return NotFound(result);
        }

        // Endpoint xóa tác giả
        [HttpDelete("delete_author/{idAuthor}")]
        public async Task<IActionResult> deleteAuthor(Guid idAuthor)
        {
            var result = await _authorService.deleteAuthorAsync(idAuthor);
            if (result.Success)
                return Ok(result);
            return NotFound(result);
        }

        [HttpGet("findAuthor")]
        [Authorize]
        public async Task<IActionResult> findAuthor(string name)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(user)) return NotFound("Không tìm thấy tài khoản"); 
            var result = await _authorService.findAuthor(new FindAuthorInputDto { nameAuthor = name});
            return Ok(result);
        }
        [HttpGet("getauthorbyid{idauthor}")]
        public async Task<IActionResult> getAuthorById(Guid idauthor)
        {
            return Ok(await _authorService.GetAuthorById(idauthor));
        }
    }
}
