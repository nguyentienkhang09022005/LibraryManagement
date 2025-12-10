using LibraryManagement.Dto.Request;
using LibraryManagement.Service.Interface;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        [HttpPost("add-role-permission")]
        public async Task<IActionResult> addNewRolePermission([FromBody] RolePermissionRequest request)
        {
            var result = await _rolePermissionService.addRolePermissionAsync(request);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        // Endpoint xóa phân quyền
        [Authorize]
        [HttpDelete("delete-role-permission")]
        public async Task<IActionResult> deleteRolePermission([FromBody] RolePermissionRequest request)
        {
            var result = await _rolePermissionService.deleteRolePermissionAsync(request);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        // Endpoint thay đổi phân quyền
        [Authorize]
        [HttpPatch("update-role-permission")]
        public async Task<IActionResult> updateRolePermission([FromBody] RolePermissionUpdateRequest request)
        {
            var result = await _rolePermissionService.updateRolePermissionAsync(request);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }
    }
}
