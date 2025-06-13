using LibraryManagement.Dto.Request;
using LibraryManagement.Repository.InterFace;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TypeBookController : ControllerBase
    {
        private readonly ITypeBookService _typeBookService;

        public TypeBookController(ITypeBookService typeBookService)
        {
            _typeBookService = typeBookService;
        }

        // Endpoint thêm loại sách
        [HttpPost("add_typebook")]
        public async Task<IActionResult> addTypeBook([FromBody] TypeBookRequest request)
        {
            var result = await _typeBookService.addTypeBookAsync(request);
            if (result.Success)
                return Created("", result);
            return BadRequest(result);
        }

        // Endpoint sửa loại sách
        [HttpPut("update_typebook/{idTypeBook}")]
        public async Task<IActionResult> updateTypeBook([FromBody] TypeBookRequest request, Guid idTypeBook)
        {
            var result = await _typeBookService.updateTypeBookAsync(request, idTypeBook);
            if (result.Success)
                return Ok(result);
            return NotFound(result);
        }

        // Endpoint xóa loại sách
        [HttpDelete("delete_typebook/{idTypeBook}")]
        public async Task<IActionResult> deleteTypeBook(Guid idTypeBook)
        {
            var result = await _typeBookService.deleteTypeBook(idTypeBook);
            if (result.Success)
                return Ok(result);
            return NotFound(result);
        }
        [HttpGet("getTypeBook")]
        public async Task<IActionResult> getTypeBookAndHeader()
        {
            var result = await _typeBookService.getTypebookAndBooks();
            return Ok(result); 
        }
        [HttpGet("getAllTypeBook")]
        public async Task<IActionResult> getAlltypeBook()
        {
            return Ok(await _typeBookService.getAllTypeBook());
        }
    }
}
