using LibraryManagement.Dto.Request;
using LibraryManagement.Repository.InterFace;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [Route("api/roles/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        // Endpoint tạo role mới
        [Authorize]
        [HttpPost("add-role")]
        public async Task<IActionResult> addNewRole([FromBody] RoleRequest request)
        {
            var result = await _roleService.addRoleAsync(request);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        // Endpoint xóa role
        [Authorize]
        [HttpDelete("delete-role")]
        public async Task<IActionResult> deleteRole([FromQuery] string RoleName)
        {
            var result = await _roleService.deleteRoleAsync(RoleName);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        // Endpoint sửa role
        [Authorize]
        [HttpPut("update-role")]
        public async Task<IActionResult> updateRole([FromBody] RoleRequest request)
        {
            var result = await _roleService.updateRoleAsync(request);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpGet("get-all-role")]
        public async Task<IActionResult> getAllRoles()
        {
            var result = await _roleService.listRolesAsync();
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpGet("get-all-permisson")]
        public async Task<IActionResult> getAllPermission()
        {
            var result = await _roleService.listPerMissionAsync();
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpGet("get-all-permisson-by-role")]
        public async Task<IActionResult> getAllPermissionByRole([FromQuery] string rolename)
        {
            var result = await _roleService.listPermissionsByRoleAsync(rolename);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }
    }
}
