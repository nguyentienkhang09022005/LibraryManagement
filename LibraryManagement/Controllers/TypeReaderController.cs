using LibraryManagement.Dto.Request;
using LibraryManagement.Repository.InterFace;
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

        // Endpoint thêm loại độc giả
        [HttpPost("add_typereader")]
        public async Task<IActionResult> addTypeReader([FromBody] TypeReaderRequest request)
        {
            var result = await _typeReaderServie.addTypeReaderAsync(request);
            if (result.Success)
                return Created("", result);
            return BadRequest(result);
        }

        // Endpoint sửa loại độc giả
        [HttpPut("update_typereader/{idTypeReader}")]
        public async Task<IActionResult> updateTypeReader([FromBody] TypeReaderRequest request, Guid idTypeReader)
        {
            var result = await _typeReaderServie.updateTypeReaderAsync(request, idTypeReader);
            if (result.Success)
                return Ok(result);
            return NotFound(result);
        }

        // Endpoint xóa loại độc giả
        [HttpDelete("delete_typereader/{idTypeReader}")]
        public async Task<IActionResult> deleteTypeReader(Guid idTypeReader)
        {
            var result = await _typeReaderServie.deleteTypeReaderAsync(idTypeReader);
            if (result.Success)
                return Ok(result);
            return NotFound(result);
        }
        [HttpGet("getAllTypeReader")]
        public async Task<IActionResult> getAllTypeReader()
        {
            try
            {
                return Ok(await _typeReaderServie.getAllTypeReader());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
} 
