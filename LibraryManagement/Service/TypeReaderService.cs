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

        public async Task<ApiResponse<TypeReaderResponse>> AddTypeReaderAsync(TypeReaderRequest request)
        {
            var newTypeReader = _mapper.Map<TypeReader>(request);
            _context.TypeReaders.Add(newTypeReader);
            await _context.SaveChangesAsync();
            var typeReaderResponse = _mapper.Map<TypeReaderResponse>(newTypeReader);
            return ApiResponse<TypeReaderResponse>.SuccessResponse(
                "Thêm loại độc giả thành công!", 
                201, 
                typeReaderResponse);
        }

        public async Task<ApiResponse<string>> DeleteTypeReaderAsync(Guid idTypeReader)
        {
            var deleteTypeReader = await _context.TypeReaders.FirstOrDefaultAsync(typereader => typereader.IdTypeReader == idTypeReader);
            if (deleteTypeReader == null)
            {
                return ApiResponse<string>.FailResponse("Không tìm thấy loại độc giả!", 404);
            }
            _context.TypeReaders.Remove(deleteTypeReader);
            await _context.SaveChangesAsync();
            return ApiResponse<string>.SuccessResponse(
                "Đã xóa loại độc giả!", 
                200, 
                string.Empty);
        }

        public async Task<ApiResponse<List<TypeReaderResponse>>> GetAllTypeReader()
        {
            var result = await _context.TypeReaders.AsNoTracking()
                .Select(x => new TypeReaderResponse
                {
                    idTypeReader = x.IdTypeReader,
                    NameTypeReader = x.NameTypeReader
                }).ToListAsync();
            return ApiResponse<List<TypeReaderResponse>>.SuccessResponse(
                "Lấy danh sách loại độc giả thành công!",
                200,
                result);
        }

        public async Task<ApiResponse<TypeReaderResponse>> UpdateTypeReaderAsync(TypeReaderRequest request, 
                                                                                 Guid idTypeReader)
        {
            var updateTypeReader = await _context.TypeReaders.FirstOrDefaultAsync(typereader => typereader.IdTypeReader == idTypeReader);
            if (updateTypeReader == null)
            {
                return ApiResponse<TypeReaderResponse>.FailResponse(
                    "Không tìm thấy loại độc giả!", 
                    404);
            }
            _mapper.Map(request, updateTypeReader);

            _context.TypeReaders.Update(updateTypeReader);
            await _context.SaveChangesAsync();
            var typeReaderResponse = _mapper.Map<TypeReaderResponse>(updateTypeReader);
            return ApiResponse<TypeReaderResponse>.SuccessResponse(
                "Thay đổi thông tin loại độc giả thành công!", 
                200, 
                typeReaderResponse);
        }
    }
}
