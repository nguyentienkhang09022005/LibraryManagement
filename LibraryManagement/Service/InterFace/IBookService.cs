using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers;
using Npgsql.EntityFrameworkCore.PostgreSQL.Internal;


namespace LibraryManagement.Repository.InterFace
{
    public interface IBookService
    {
        public Task<ApiResponse<HeaderBookResponse>> addBookAsync(HeaderBookCreationRequest request);
        public Task<ApiResponse<HeaderBookResponse>> updateBookAsync(HeaderBookUpdateRequest request, string idBook, string idTheBook);
        public Task<ApiResponse<string>> deleteBookAsync(string idBook);

      
        public Task<List<EvaluationDetails>> getBooksEvaluation(EvaluationDetailInput dto);

        public Task<bool> LikeBook(EvaluationDetailInput dto);

        //public Task<List<HeadbookAndComments>> getLikedHeaderBook(string token);

        public Task<bool> DeleteEvaluation(DeleteEvaluationInput dto);

        public Task<List<GetHeaderbookResponse>> GetAllHeaderBooks(string token);

        public Task<List<BooksAndComments>> getAllBooksInDetail(string token);

        public Task<List<BooksAndCommentsWithoutLogin>> findBook(string namebook);



    } 
}
