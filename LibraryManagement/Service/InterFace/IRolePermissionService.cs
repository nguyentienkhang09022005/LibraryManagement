using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers;

namespace LibraryManagement.Service.Interface
{
    public interface IRolePermissionService
    {
        public Task<ApiResponse<RolePermissionResponse>> addRolePermissionAsync(RolePermissionRequest request);
        public Task<ApiResponse<string>> deleteRolePermissionAsync(RolePermissionRequest request);

        public Task<ApiResponse<RolePermissionResponse>> updateRolePermissionAsync(RolePermissionUpdateRequest request);
    }
}
