using LibraryManagement.Dto.Request;
using LibraryManagement.Repository.InterFace;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TypeReaderController : ControllerBase
    {
        private readonly ITypeReaderService _typeReaderServie;

        public TypeReaderController(ITypeReaderService typeReaderServie)
        {
            _typeReaderServie = typeReaderServie;
        }

        [Authorize]
        [HttpPost("add-typereader")]
        public async Task<IActionResult> addTypeReader([FromBody] TypeReaderRequest request)
        {
            var result = await _typeReaderServie.AddTypeReaderAsync(request);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpPut("update-typereader")]
        public async Task<IActionResult> updateTypeReader([FromBody] TypeReaderRequest request, 
                                                          [FromQuery] Guid idTypeReader)
        {
            var result = await _typeReaderServie.UpdateTypeReaderAsync(request, idTypeReader);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpDelete("delete-typereader")]
        public async Task<IActionResult> deleteTypeReader([FromQuery] Guid idTypeReader)
        {
            var result = await _typeReaderServie.DeleteTypeReaderAsync(idTypeReader);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpGet("get-all-typereader")]
        public async Task<IActionResult> getAllTypeReader()
        {
            var result = await _typeReaderServie.GetAllTypeReader();
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }
    }
} 
