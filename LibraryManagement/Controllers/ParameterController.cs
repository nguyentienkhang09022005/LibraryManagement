using LibraryManagement.Dto.Request;
using LibraryManagement.Repository.InterFace;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [Authorize]
        [HttpPost("add-parameter")]
        public async Task<IActionResult> addNewParameter([FromBody] ParameterRequest request)
        {
            var result = await _parameterService.addParameterAsync(request);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpPut("update-parameter")]
        public async Task<IActionResult> updateParameter([FromBody] ParameterRequest request, [FromQuery] Guid idParameter)
        {
            var result = await _parameterService.updateParameterAsync(request, idParameter);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpDelete("delete-parameter")]
        public async Task<IActionResult> deleteParameter([FromQuery] Guid idParameter)
        {
            var result = await _parameterService.deleteParameterAsync(idParameter);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpGet("get-all-parameter")]
        public async Task<IActionResult> getAllParameter()
        {
            var result = await _parameterService.getParametersAsync();
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }
    }
}
