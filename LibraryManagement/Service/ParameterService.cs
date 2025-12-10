using AutoMapper;
using LibraryManagement.Data;
using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers;
using LibraryManagement.Models;
using LibraryManagement.Repository.InterFace;
using Microsoft.EntityFrameworkCore;
using SendGrid;

namespace LibraryManagement.Repository
{
    public class ParameterService : IParameterService
    {
        private readonly LibraryManagermentContext _context;
        private readonly IMapper _mapper;

        public ParameterService(LibraryManagermentContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Hàm thêm quy định
        public async Task<ApiResponse<ParameterResponse>> addParameterAsync(ParameterRequest request)
        {
            // Chuyển về in thường và xóa khoảng cách
            var normalInput = request.NameParameter.ToLowerInvariant().Replace(" ", "");
            var param = _context.Parameters.AsEnumerable().FirstOrDefault(pr => pr.NameParameter.ToLowerInvariant().Replace(" ", "") == normalInput);

            Parameter parameter;
            if (param != null)
            {
                param.ValueParameter = request.ValueParameter;
                parameter = param;
            }else
            {
                parameter = _mapper.Map<Parameter>(request);
                _context.Parameters.Add(parameter);
            }
            await _context.SaveChangesAsync();
            var response = _mapper.Map<ParameterResponse>(parameter);

            return ApiResponse<ParameterResponse>.SuccessResponse("Thêm quy định thành công!", 201, response);
        }

        // Xóa quy định
        public async Task<ApiResponse<string>> deleteParameterAsync(Guid idParameter)
        {
            var param = await _context.Parameters.FirstOrDefaultAsync(pr => pr.IdParameter == idParameter);
            if (param == null)
            {
                return ApiResponse<string>.FailResponse("Không tìm thấy quy định!", 404);
            }
            _context.Parameters.Remove(param);
            await _context.SaveChangesAsync();
            return ApiResponse<string>.SuccessResponse("Đã xóa quy định!", 200, string.Empty);
        }

        // Sửa quy định
        public async Task<ApiResponse<ParameterResponse>> updateParameterAsync(ParameterRequest request, Guid idParameter)
        {
            var param = await _context.Parameters.FirstOrDefaultAsync(pr => pr.IdParameter == idParameter);
            if (param == null)
            {
                return ApiResponse<ParameterResponse>.FailResponse("Không tìm thấy quy định!", 404);
            }
            _mapper.Map(request, param);
            await _context.SaveChangesAsync();
            var response = _mapper.Map<ParameterResponse>(param);

            return ApiResponse<ParameterResponse>.SuccessResponse("Thay đổi quy định thành công!", 201, response);
        }

        // Lấy giá trị của các quy định 
        public async Task<int> getValueAsync(string nameParameter)
        {
            var normalInput = nameParameter.ToLowerInvariant().Replace(" ", "");
            var param = _context.Parameters.AsEnumerable().FirstOrDefault(pr => pr.NameParameter.ToLowerInvariant().Replace(" ", "") == normalInput);
            if (param == null)
            {
                throw new Exception($"Không tìm thấy quy định: {nameParameter}!");
            }
            return param.ValueParameter;
        }

        public async Task<ApiResponse<List<Parameter>>> getParametersAsync()
        {
           var result = await _context.Parameters.AsNoTracking().ToListAsync();
            return ApiResponse<List<Parameter>>.SuccessResponse(
                "Lấy danh sách quy định thành công!", 
                201, 
                result);
        }
    }
}
