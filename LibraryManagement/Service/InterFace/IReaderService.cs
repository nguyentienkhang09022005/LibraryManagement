using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers;

namespace LibraryManagement.Repository.IRepository
{
    public interface IReaderService
    {
        Task<ApiResponse<ReaderResponse>> addReaderAsync(ReaderCreationRequest request);

        Task<ApiResponse<List<ReaderResponse>>> getAllReaderAsync();

        Task<ApiResponse<ReaderResponse>> updateReaderAsync(ReaderUpdateRequest request, string idReader);

        Task<ApiResponse<string>> deleteReaderAsync(string idReader);

        Task<ApiResponse<FindReaderResponse>> findReaderAsync(string dto);

        Task<string> generateNextIdReaderAsync();

        Task<ApiResponse<FindReaderResponse>> findReaderInputAsync(string idReader);

        Task<string?> GetManagerIdAsync();
    }
}
