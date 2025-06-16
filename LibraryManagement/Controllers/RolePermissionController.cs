using LibraryManagement.Dto.Request;
using LibraryManagement.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolePermissionController : ControllerBase
    {
        private readonly IRolePermissionService _rolePermissionService;
        public RolePermissionController(IRolePermissionService rolePermissionService)
        {
            _rolePermissionService = rolePermissionService;
        }

        // Endpoint phân quyền mới
        [HttpPost("add_role_permission")]
        public async Task<IActionResult> addNewRolePermission([FromBody] RolePermissionRequest request)
        {
            var result = await _rolePermissionService.addRolePermissionAsync(request);
            if (result.Success)
                return Created("", result);
            return BadRequest(result);
        }

        // Endpoint xóa phân quyền
        [HttpDelete("delete_role_permission")]
        public async Task<IActionResult> deleteRolePermission([FromBody] RolePermissionRequest request)
        {
            var result = await _rolePermissionService.deleteRolePermissionAsync(request);
            if (result.Success)
                return Ok(result);
            return NotFound(result);
        }

        // Endpoint thay đổi phân quyền
        [HttpPatch("update_role_permission")]
        public async Task<IActionResult> updateRolePermission([FromBody] RolePermissionUpdateRequest request)
        {
            var result = await _rolePermissionService.updateRolePermissionAsync(request);
            if (result.Success)
                return Ok(result);
            return NotFound(result);
        }
    }
}
