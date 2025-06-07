using LibraryManagement.Dto.Request;
using LibraryManagement.Repository.InterFace;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Formats.Asn1;
using System.Security.Claims;

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
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
        public async Task<IActionResult> getDetailedEvaluation(EvaluationDetailInput dto)
        {
            var result = await _bookService.getBooksEvaluation(dto);
            if (result == null) return Unauthorized("Không có quyền admin");
            return Ok(result);
        }

        [HttpPost("LikeHeaderBook")]
        public async Task<IActionResult> likeHeaderBook(EvaluationDetailInput dto)
        {
            var result = await _bookService.LikeBook(dto);
            if (result == false) return Unauthorized("Vui lòng đăng nhập ");
            return Ok("Success");
        }
        //[HttpPost("getAllBooksAndComments")]
        //public async Task<IActionResult> getAllBooksAndComments([FromBody] string token)
        //{
        //    var result = await _bookService.getAllHeaderbookandComments(token);
        //    if (result == null) return Unauthorized("Vui lòng đăng nhập");
        //    return Ok(result); 
        //}

        //[HttpPost("getLikedHeaderbook")]
        //public async Task<IActionResult> getLikeHeaderBook([FromBody] string token)
        //{
        //    var result = await _bookService.getLikedHeaderBook(token);
        //    if (result == null) return Unauthorized("Vui lòng đăng nhập");
        //    return Ok(result); 

        //}
        [HttpDelete("deleteEvaluation")]
        public async Task<IActionResult> deleteEvaluation([FromBody]DeleteEvaluationInput dto)
        {
            var user = await _bookService.DeleteEvaluation(dto);
            if (user == false) return Unauthorized();
            return Ok("Xóa thành công"); 
        }

        [HttpPost("getallheaderbooks")]
        public async Task<IActionResult> getAllHeaderbooks([FromBody] string token)
        {
            var result = await _bookService.GetAllHeaderBooks(token);
            return (result == null) ? Unauthorized("Vui lòng đăng nhập") : Ok(result); 
        }
        [HttpPost("getbooksandcomments")]
        public async Task<IActionResult> getBooksAndComments([FromBody] string token)
        {
            var result = await _bookService.getAllBooksInDetail(token);
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
