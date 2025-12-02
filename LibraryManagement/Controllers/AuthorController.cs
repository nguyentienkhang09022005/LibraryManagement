using LibraryManagement.Dto.Request;
using LibraryManagement.Repository.InterFace;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpGet("list-author")]
        public async Task<IActionResult> gettListAuthor()
        {
            var result = await _authorService.GetListAuthor();
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("add-author")]
        public async Task<IActionResult> addAuthor([FromForm] AuthorCreationRequest request)
        {
            var result = await _authorService.AddAuthorAsync(request);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpPatch("update-author")]
        public async Task<IActionResult> updateAuthor([FromForm] AuthorUpdateRequest request, 
                                                      [FromQuery] Guid idAuthor)
        {
            var result = await _authorService.UpdateAuthorAsync(request, idAuthor);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("delete-author")]
        public async Task<IActionResult> deleteAuthor([FromQuery] Guid idAuthor)
        {
            var result = await _authorService.DeleteAuthorAsync(idAuthor);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("find-author-by-name")]
        public async Task<IActionResult> findAuthor([FromQuery] AuthorFindNameRequest request)
        {
            var result = await _authorService.FindAuthor(request);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("inf-author")]
        public async Task<IActionResult> getAuthorById([FromQuery] Guid idAuthor)
        {
            var result = await _authorService.GetAuthorById(idAuthor);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }
    }
}
