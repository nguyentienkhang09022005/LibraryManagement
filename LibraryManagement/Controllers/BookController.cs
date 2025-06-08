using LibraryManagement.Data;
using LibraryManagement.Dto.Request;
using LibraryManagement.Repository.InterFace;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Formats.Asn1;
using System.Runtime.InteropServices;
using System.Security.Claims;
using ZstdSharp.Unsafe;

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly LibraryManagermentContext _context;
        public BookController(IBookService bookService, LibraryManagermentContext context)
        {
            _bookService = bookService;
            _context = context;
        }

        // Endpoint tạo sách
        [HttpPost("add_book")]
        public async Task<IActionResult> addHeaderBook([FromForm] HeaderBookCreationRequest request)
        {
            var result = await _bookService.addBookAsync(request);
            if (result.Success)
                return Created("", result);
            return BadRequest(result);
        }

        // Endpoint xóa sách
        [HttpDelete("delete_book/{idBook}/{idTheBook}")]
        public async Task<IActionResult> deleteHeaderBook(string idBook)
        {
            var result = await _bookService.deleteBookAsync(idBook);
            if (result.Success)
                return Ok(result);
            return NotFound(result);
        }

        // Endpoint sửa sách
        [HttpPut("update_book/{idBook}/{idTheBook}")]
        public async Task<IActionResult> updateHeaderBook([FromForm] HeaderBookUpdateRequest request, 
                                                          string idBook, 
                                                          string idTheBook)
        {
            var result = await _bookService.updateBookAsync(request, idBook, idTheBook);
            if (result.Success)
                return Ok(result);
            return NotFound(result);
        }


        //[HttpPost("getBookAndCommentsByid")]
        //public async Task<IActionResult> getBooksAndCommentbyId([FromBody] GetHeaderBookDtoInput dto)
        //{
        //    var result = await _bookService.getHeaderbookandCommentsByid(dto);
        //    return Ok(result); 
        //}

        [HttpPost("getEvaluation")]
        [Authorize]
        public async Task<IActionResult> getDetailedEvaluation(string idBook)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return NotFound("Không tìm thấy thông tin người dùng");
            var result = await _bookService.getBooksEvaluation(new EvaluationDetailInput { idUser = userId, IdBook = idBook });
          
            return Ok(result);
        }

        [HttpPost("LikeBook")]
        [Authorize]
        public async Task<IActionResult> LikeBook(string idBook)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _bookService.LikeBook(new EvaluationDetailInput { idUser = userId, IdBook = idBook});
            return Ok(result);
        }

        [HttpGet("getlikedbook")]
        [Authorize]
        public async Task<IActionResult> getLikeHeaderBook()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null || string.IsNullOrEmpty(userId)) return NotFound("Không tìm thấy thông tin người dùng"); 
            var result = await _bookService.getFavoriteBook(userId);
            return Ok(result);

        }
        [HttpDelete("deleteEvaluation")]
        public async Task<IActionResult> deleteEvaluation([FromBody]DeleteEvaluationInput dto)
        {
            var user = await _bookService.DeleteEvaluation(dto);
            if (user == false) return Unauthorized();
            return Ok("Xóa thành công"); 
        }

        [HttpPost("getallheaderbooks")]
        [Authorize]
        public async Task<IActionResult> getAllHeaderbooks()
        {
            var result = await _bookService.GetAllHeaderBooks();
            return (result == null) ? Unauthorized("Vui lòng đăng nhập") : Ok(result); 
        }
        [HttpGet("getbooksindetail")]
        [Authorize]
        public async Task<IActionResult> getBooksAndComments()
        {
            var userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userID)) return NotFound("Không tìm thấy thông tin người dùng");
            var result = await _bookService.getAllBooksInDetail(userID);
            return (result == null) ? Unauthorized("Vui lòng đăng nhập") : Ok(result); 
        }
        [HttpGet("getbooksindetailbyid{idbook}")]
   
        public async Task<IActionResult> getBooksAndCommentsById(string idbook)
        {
            var result = await _bookService.getAllBooksInDetailById(idbook);
            return (result == null) ? Unauthorized("Vui lòng đăng nhập") : Ok(result);
        }

        [HttpGet("findBooks{namebook}")]
        public async Task<IActionResult> findBooks(string namebook)
        {
            var result = await _bookService.findBook(namebook);
            return Ok(result);
        }
    }
}
