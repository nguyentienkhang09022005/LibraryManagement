using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers;


namespace LibraryManagement.Repository.InterFace
{
    public interface IBookService
    {
        public Task<ApiResponse<HeaderBookResponse>> addBookAsync(HeaderBookCreationRequest request);
        public Task<ApiResponse<HeaderBookUpdateResponse>> updateBookAsync(HeaderBookUpdateRequest request, string idBook);
        public Task<ApiResponse<string>> deleteBookAsync(string idBook);
        public Task<ApiResponse<ChangeStatusOfTheBookResponse>> changeStatusOfTheBookAsync(ChangeStatusOfTheBookRequest request);

        public Task<List<EvaluationDetails>> getBooksEvaluation(EvaluationDetailInput dto);
        
        public Task<ApiResponse<bool>> LikeBook(EvaluationDetailInput dto);

        public Task<List<BooksAndComments>> getFavoriteBook(string idUser);

        public Task<bool> DeleteEvaluation(DeleteEvaluationInput dto);

        public Task<List<GetHeaderbookResponse>> GetAllHeaderBooks();

        public Task<List<GetHeaderbookResponse>> GetAllHeaderBooksByTheBook(string idThebook);

        public Task<List<BooksAndComments>> getAllBooksInDetail(string readerId);


        public Task<List<BooksAndCommentsWithoutLogin>> findBook(string namebook);
        public Task<List<BooksAndCommentsWithoutLogin>> getAllBooksInDetailById(string idbook);

        public Task<ApiResponse<bool>> addEvaluation(string idreader,AddEvaluation dto);

        public Task<List<CommentResponse>> getAllCommentByIdBook(string idbook);

        public Task<List<EvaResponse>> getAllStar(string idbook);

    } 
}
