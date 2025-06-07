using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers;

namespace LibraryManagement.Repository.IRepository
{
    public interface IReaderService
    {
        // Interface thêm độc giả
        public Task<ApiResponse<ReaderResponse>> addReaderAsync(ReaderCreationRequest request);

        // Interface lấy danh sách độc giả
        public Task<List<ReaderResponse>> getAllReaderAsync();

        // Interface sửa độc giả
        public Task<ApiResponse<ReaderResponse>> updateReaderAsync(ReaderUpdateRequest request, string idReader);

        // Interface xóa độc giả
        public Task<ApiResponse<string>> deleteReaderAsync(string idReader);

        public Task<FindReaderOutputDto> findReaderAsync(string dto);
        public Task<string> generateNextIdReaderAsync();
    }
}
