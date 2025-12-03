using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers;

namespace LibraryManagement.Repository.InterFace
{
    public interface IBookReceiptService
    {
        Task<ApiResponse<BooKReceiptResponse>> AddBookReceiptAsync(BookReceiptRequest request);

        Task<ApiResponse<string>> DeleteBookReceiptAsync(Guid idBookReipt);

        Task<string> generateNextIdBookAsync();

        Task<string> generateNextIdTheBookAsync();

        Task<ApiResponse<List<BookReceiptInformationOutput>>> GetAllReceiptHistory(string token);
    }
}
