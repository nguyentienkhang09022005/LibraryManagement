using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers;

namespace LibraryManagement.Repository.IRepository
{
    public interface IReaderService
    {
        public Task<ApiResponse<ReaderResponse>> addReaderAsync(ReaderCreationRequest request);

        Task<ApiResponse<List<ReaderResponse>>> getAllReaderAsync();

        public Task<ApiResponse<ReaderResponse>> updateReaderAsync(ReaderUpdateRequest request, string idReader);

        public Task<ApiResponse<string>> deleteReaderAsync(string idReader);

        Task<ApiResponse<FindReaderResponse>> findReaderAsync(string dto);

        public Task<string> generateNextIdReaderAsync();

        Task<ApiResponse<FindReaderResponse>> findReaderInputAsync(string idReader);
    }
}
