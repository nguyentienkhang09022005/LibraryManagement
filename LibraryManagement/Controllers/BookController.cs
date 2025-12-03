using LibraryManagement.Data;
using LibraryManagement.Dto.Request;
using LibraryManagement.Repository.InterFace;
using Microsoft.AspNetCore.Mvc;

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
        [HttpPost("add-book")]
        public async Task<IActionResult> addHeaderBook([FromForm] HeaderBookCreationRequest request)
        {
            var result = await _bookService.AddBookAsync(request);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        // Endpoint xóa sách
        [HttpDelete("delete-book")]
        public async Task<IActionResult> deleteHeaderBook([FromQuery] string idBook)
        {
            var result = await _bookService.DeleteBookAsync(idBook);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        // Endpoint sửa sách
        [HttpPatch("update-book")]
        public async Task<IActionResult> updateHeaderBook([FromForm] HeaderBookUpdateRequest request, 
                                                          [FromQuery] string idBook)
        {
            var result = await _bookService.UpdateBookAsync(request, idBook);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        // Endpoint thay đổi trạng thái cuốn sách
        [HttpPatch("change-status")]
        public async Task<IActionResult> changeStatusOfTheBook(ChangeStatusOfTheBookRequest request)
        {
            var result = await _bookService.ChangeStatusOfTheBookAsync(request);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("detail-evaluation")]
        public async Task<IActionResult> getDetailedEvaluation([FromBody] EvaluationRequest evaluationRequest)
        {
            var result = await _bookService.GetBooksEvaluation(evaluationRequest);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("like-book")]
        public async Task<IActionResult> LikeBook([FromBody] EvaluationRequest evaluationRequest)
        {
            var result = await _bookService.LikeBook(evaluationRequest);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("list-liked-book")]
        public async Task<IActionResult> getLikeHeaderBook([FromQuery] string idReader)
        {
            var result = await _bookService.GetFavoriteBook(idReader);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);

        }

        [HttpDelete("delete-evaluation")]
        public async Task<IActionResult> deleteEvaluation([FromBody] DeleteEvaluationRequest request)
        {
            var result = await _bookService.DeleteEvaluation(request);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("list-headerbook")]
        public async Task<IActionResult> getAllHeaderbooks()
        {
            var result = await _bookService.GetAllHeaderBooks();
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("books-in-detail")]
        public async Task<IActionResult> getBooksAndComments([FromQuery] string idUser)
        {
            var result = await _bookService.GetAllBooksInDetail(idUser);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("books-in-detail-by-id")]
        public async Task<IActionResult> getBooksAndCommentsById([FromQuery] string idbook)
        {
            var result = await _bookService.GetAllBooksInDetailById(idbook);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("find-books")]
        public async Task<IActionResult> findBooks([FromQuery] string namebook)
        {
            var result = await _bookService.FindBook(namebook);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("headerbook-by-thebook-id")]
        public async Task<IActionResult> getHeaderBookByThebookId([FromQuery] string idTheBook)
        {
            var result = await _bookService.GetAllHeaderBooksByTheBook(idTheBook);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("add-evaluation")]
        public async Task<IActionResult> addEvaluation([FromBody] AddEvaluationRequest request, [FromQuery] string idUser)
        {
            var result = await _bookService.AddEvaluation(idUser, request);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("all-comments")]
        public async Task<IActionResult> getAllComments([FromQuery] string idBook)
        {
            var result = await _bookService.GetAllCommentByIdBook(idBook);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("star-by-id")]
        public async Task<IActionResult> getStarByid([FromQuery] string idbook)
        {
            var result = await _bookService.GetAllStar(idbook);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("edit-comment")]
        public async Task<IActionResult> editComment(string idComment, string comment, int rate)
        {
            var result = await _bookService.EditCommentAsync(idComment, comment, rate);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("delete-comment")]
        public async Task<IActionResult> deleteComment([FromQuery] string idComment, [FromQuery] string idReader)
        {
            var result = await _bookService.DeleteComment(idComment, idReader);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("the-book-status")]
        public async Task<IActionResult> GetTheBookStatus([FromQuery] string idThebook)
        {
            var result = await _bookService.GetTheBookStatus(idThebook);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }
    }
}
