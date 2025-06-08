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
        
        public Task<ApiResponse<bool>> LikeBook(EvaluationDetailInput dto);

        public Task<List<BooksAndComments>> getFavoriteBook(string idUser);

        public Task<bool> DeleteEvaluation(DeleteEvaluationInput dto);

        public Task<List<GetHeaderbookResponse>> GetAllHeaderBooks();

        public Task<List<BooksAndComments>> getAllBooksInDetail(string readerId);


        public Task<List<BooksAndCommentsWithoutLogin>> findBook(string namebook);
        public Task<List<BooksAndCommentsWithoutLogin>> getAllBooksInDetailById(string idbook);


    } 
}
