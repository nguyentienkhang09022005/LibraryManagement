using AutoMapper;
using LibraryManagement.Data;
using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers;
using LibraryManagement.Models;
using LibraryManagement.Repository.InterFace;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Repository
{
    public class TypeReaderService : ITypeReaderService
    {
        private readonly LibraryManagermentContext _context;
        private readonly IMapper _mapper;

        public TypeReaderService(LibraryManagermentContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Hàm thêm loại độc giả
        public async Task<ApiResponse<TypeReaderResponse>> addTypeReaderAsync(TypeReaderRequest request)
        {
            var newTypeReader = _mapper.Map<TypeReader>(request);
            _context.TypeReaders.Add(newTypeReader);
            await _context.SaveChangesAsync();
            var typeReaderResponse = _mapper.Map<TypeReaderResponse>(newTypeReader);
            return ApiResponse<TypeReaderResponse>.SuccessResponse("Thêm loại độc giả thành công", 201, typeReaderResponse);
        }

        // Hàm xóa loại độc giả
        public async Task<ApiResponse<string>> deleteTypeReaderAsync(Guid idTypeReader)
        {
            var deleteTypeReader = await _context.TypeReaders.FirstOrDefaultAsync(typereader => typereader.IdTypeReader == idTypeReader);
            if (deleteTypeReader == null)
            {
                return ApiResponse<string>.FailResponse("Không tìm thấy loại độc giả", 404);
            }
            _context.TypeReaders.Remove(deleteTypeReader);
            await _context.SaveChangesAsync();
            return ApiResponse<string>.SuccessResponse("Đã xóa loại độc giả", 200, "");
        }

     

        // Hàm sửa loại độc giả
        public async Task<ApiResponse<TypeReaderResponse>> updateTypeReaderAsync(TypeReaderRequest request, Guid idTypeReader)
        {
            var updateTypeReader = await _context.TypeReaders.FirstOrDefaultAsync(typereader => typereader.IdTypeReader == idTypeReader);
            if (updateTypeReader == null)
            {
                return ApiResponse<TypeReaderResponse>.FailResponse("Không tìm thấy loại độc giả", 404);
            }
            _mapper.Map(request, updateTypeReader);

            _context.TypeReaders.Update(updateTypeReader);
            await _context.SaveChangesAsync();
            var typeReaderResponse = _mapper.Map<TypeReaderResponse>(updateTypeReader);
            return ApiResponse<TypeReaderResponse>.SuccessResponse("Thay đổi thông tin loại độc giả thành công", 200, typeReaderResponse);
        }
    }
}
