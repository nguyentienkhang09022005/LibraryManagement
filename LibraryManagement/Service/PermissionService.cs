using LibraryManagement.Data;
using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers;
using LibraryManagement.Models;
using LibraryManagement.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Service
{
    public class PermissionService : IPermissionService
    {
        private readonly LibraryManagermentContext _context;

        public PermissionService(LibraryManagermentContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<PermissionResponse>> addPermissionAsync(PermissionRequest request)
        {
            var permission = await _context.Permissions.FirstOrDefaultAsync(p => p.PermissionName == request.PermissionName);
            if (permission != null)
            {
                return ApiResponse<PermissionResponse>.FailResponse("Permission đã tồn tại!", 400);
            }

            permission = new Permission
            {
                PermissionName = request.PermissionName,
                Description = request.Description
            };
            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();
            return ApiResponse<PermissionResponse>.SuccessResponse("Tạo permission thành công!", 200, new PermissionResponse
            {
                PermissionName = permission.PermissionName,
                Description = permission.Description
            });
        }

        public async Task<ApiResponse<string>> deletePermissionAsync(string permissionName)
        {
            var permission = await _context.Permissions.FirstOrDefaultAsync(p => p.PermissionName == permissionName);
            if (permission == null)
            {
                return ApiResponse<string>.FailResponse("Permission không tồn tại!", 404);
            }

            _context.Permissions.Remove(permission);
            await _context.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse("Xóa permission thành công!", 200, permissionName);
        }

        public async Task<ApiResponse<PermissionResponse>> updatePermissionAsync(PermissionRequest request)
        {
            var permission = await _context.Permissions.FirstOrDefaultAsync(p => p.PermissionName == request.PermissionName);
            if (permission == null)
            {
                return ApiResponse<PermissionResponse>.FailResponse("Permission không tồn tại!", 404);
            }

            permission.PermissionName = request.PermissionName;
            permission.Description = request.Description;

            _context.Permissions.Update(permission);
            await _context.SaveChangesAsync();

            return ApiResponse<PermissionResponse>.SuccessResponse("Cập nhật permission thành công!", 200, new PermissionResponse
            {
                PermissionName = permission.PermissionName,
                Description = permission.Description
            });
        }
    }
}
