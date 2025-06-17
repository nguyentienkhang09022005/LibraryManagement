using LibraryManagement.Dto.Request;
using LibraryManagement.Repository.InterFace;
using Microsoft.AspNetCore.Mvc;
using ZstdSharp.Unsafe;

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParameterController : ControllerBase
    {
        private readonly IParameterService _parameterService;
        public ParameterController(IParameterService parameterService)
        {
            _parameterService = parameterService;
        }

        // Endpoint thêm quy định
        [HttpPost("add_parameter")]
        public async Task<IActionResult> addNewParameter([FromBody] ParameterRequest request)
        {
            var result = await _parameterService.addParameterAsync(request);
            if (result.Success)
                return Created("", result);
            return BadRequest(result);
        }

        // Endpont sửa quy định
        [HttpPut("update_parameter/{idParameter}")]
        public async Task<IActionResult> updateParameter([FromBody] ParameterRequest request, Guid idParameter)
        {
            var result = await _parameterService.updateParameterAsync(request, idParameter);
            if (result.Success)
                return Ok(result);
            return NotFound(result);
        }

        // Endpoint xóa quy định
        [HttpDelete("delete_parameter/{idParameter}")]
        public async Task<IActionResult> deleteParameter(Guid idParameter)
        {
            var result = await _parameterService.deleteParameterAsync(idParameter);
            if (result.Success)
                return Ok(result);
            return NotFound(result);
        }
        [HttpGet("getallparameter")]
        public async Task<IActionResult> getAllParameter()
        {
            try
            {
                var result = await _parameterService.getParametersAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
