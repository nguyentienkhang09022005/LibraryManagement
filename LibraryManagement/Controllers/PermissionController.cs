using LibraryManagement.Dto.Request;
using LibraryManagement.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        // Endpoint tạo permisison mới
        [Authorize]
        [HttpPost("add-permission")]
        public async Task<IActionResult> addNewPermission([FromBody] PermissionRequest request)
        {
            var result = await _permissionService.addPermissionAsync(request);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        // Endpoint xóa permisison
        [Authorize]
        [HttpDelete("delete_permission")]
        public async Task<IActionResult> deletePermission([FromQuery] string permissionName)
        {
            var result = await _permissionService.deletePermissionAsync(permissionName);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        // Endpoint sửa permission
        [Authorize]
        [HttpPut("update-permission")]
        public async Task<IActionResult> updatePermission([FromBody] PermissionRequest request)
        {
            var result = await _permissionService.updatePermissionAsync(request);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }
    }
}
