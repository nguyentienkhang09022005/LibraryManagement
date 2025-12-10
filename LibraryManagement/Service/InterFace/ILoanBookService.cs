using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers;

namespace LibraryManagement.Repository.InterFace
{
    public interface ILoanBookService
    {
        Task<ApiResponse<LoanBookResponse>> addLoanBookAsync(LoanBookRequest request);

        Task<ApiResponse<string>> deleteLoanBookAsync(Guid idLoanSlipBook);

        Task<ApiResponse<List<LoanSlipBookResponse>>> getListLoanSlipBook();

        Task<ApiResponse<List<LoanBookHistory>>> getLoanSlipBookByUser(string idReader);

        Task<ApiResponse<List<GetLoanSlipBookByType>>> getLoanSlipBookByType(string genre);

        Task<ApiResponse<List<AmountOfEachTypeBook>>> getAmountByTypeBook(int month);

        Task<ApiResponse<List<LoanSlipBookResponse>>> getLoanSlipBookByReader(string idReader);
    }
}
