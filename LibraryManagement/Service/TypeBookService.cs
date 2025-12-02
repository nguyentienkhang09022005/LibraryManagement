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
    public class TypeBookService : ITypeBookService
    {
        private readonly LibraryManagermentContext _context;
        private readonly IMapper _mapper;

        public TypeBookService(LibraryManagermentContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiResponse<TypeBookResponse>> AddTypeBookAsync(TypeBookRequest request)
        {
            var newTypeBook = _mapper.Map<TypeBook>(request);
            _context.TypeBooks.Add(newTypeBook);
            await _context.SaveChangesAsync();
            var typeBookResponse = _mapper.Map<TypeBookResponse>(newTypeBook);
            return ApiResponse<TypeBookResponse>.SuccessResponse(
                "Thêm loại sách thành công!", 
                201, 
                typeBookResponse);
        }

        public async Task<ApiResponse<string>> DeleteTypeBook(Guid idTypeBook)
        {
            var deleteTypeBook = await _context.TypeBooks.FirstOrDefaultAsync(typebook => typebook.IdTypeBook == idTypeBook);
            if (deleteTypeBook == null)
            {
                return ApiResponse<string>.FailResponse("Không tìm thấy loại sách!", 404);
            }
            _context.TypeBooks.Remove(deleteTypeBook);
            await _context.SaveChangesAsync();
            return ApiResponse<string>.SuccessResponse("Đã xóa loại sách!", 200, string.Empty);
        }

        public async Task<ApiResponse<List<TypeBookResponseAndBook>>> GetTypebookAndHeaders()
        {
            var result = await _context.TypeBooks
                 .AsNoTracking()
                 .Include(typebook => typebook.HeaderBooks)
                 .SelectMany(tb => tb.HeaderBooks.Select(hb => new TypeBookResponseAndBook
                 {
                     IdHeaderBook = hb.IdHeaderBook.ToString(),
                     TypeBook = hb.TypeBook.NameTypeBook,
                     NameHeaderBook = hb.NameHeaderBook.ToString(),
                     IDTypeBook = tb.IdTypeBook.ToString()
                 }))
             .ToListAsync();
            return ApiResponse<List<TypeBookResponseAndBook>>.SuccessResponse(
                "Lấy danh sách loại sách và đầu sách thành công!", 
                200, 
                result);
        }

        public async Task<ApiResponse<TypeBookResponse>> UpdateTypeBookAsync(TypeBookRequest request, Guid idTypeBook)
        {
            var updateTypeBook = await _context.TypeBooks.FirstOrDefaultAsync(typebook => typebook.IdTypeBook == idTypeBook);
            if (updateTypeBook == null)
            {
                return ApiResponse<TypeBookResponse>.FailResponse("Không tìm thấy loại sách!", 404);
            }
            _mapper.Map(request, updateTypeBook);

            _context.TypeBooks.Update(updateTypeBook);
            await _context.SaveChangesAsync();
            var typeBookResponse = _mapper.Map<TypeBookResponse>(updateTypeBook);
            return ApiResponse<TypeBookResponse>.SuccessResponse(
                "Thay đổi thông tin loại sách thành công!", 
                200, 
                typeBookResponse);
        }

        public async Task<ApiResponse<List<TypeBookResponse>>> GetAllTypeBook()
        {
            var result = await _context.TypeBooks
                .Select(x => new TypeBookResponse { 
                    IdTypeBook = x.IdTypeBook, 
                    NameTypeBook = x.NameTypeBook})
                .ToListAsync();
            return ApiResponse<List<TypeBookResponse>>.SuccessResponse(
                "Thay đổi thông tin loại sách thành công!",
                200,
                result);
        }
    }
}
