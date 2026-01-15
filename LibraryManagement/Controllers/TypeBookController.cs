using LibraryManagement.Dto.Request;
using LibraryManagement.Repository.InterFace;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [HttpPost("add-typebook")]
        public async Task<IActionResult> addTypeBook([FromBody] TypeBookRequest request)
        {
            var result = await _typeBookService.AddTypeBookAsync(request);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpPut("update-typebook")]
        public async Task<IActionResult> updateTypeBook([FromBody] TypeBookRequest request, 
                                                        [FromQuery] Guid idTypeBook)
        {
            var result = await _typeBookService.UpdateTypeBookAsync(request, idTypeBook);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpDelete("delete-typebook")]
        public async Task<IActionResult> deleteTypeBook([FromQuery] Guid idTypeBook)
        {
            var result = await _typeBookService.DeleteTypeBook(idTypeBook);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpGet("list-typebook-and-header")]
        public async Task<IActionResult> getTypeBookAndHeaders()
        {
            var result = await _typeBookService.GetTypebookAndHeaders();
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpGet("list-all-typebook")]
        public async Task<IActionResult> getAlltypeBook()
        {
            var result = await _typeBookService.GetAllTypeBook();
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpGet("list-typebook-with-count")]
        public async Task<IActionResult> getAllTypeBookWithCount()
        {
            var result = await _typeBookService.GetAllTypeBookWithCount();
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }
    }
}
