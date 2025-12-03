using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers;

namespace LibraryManagement.Repository.InterFace
{
    public interface IBookService
    {
        public Task<ApiResponse<HeaderBookResponse>> AddBookAsync(HeaderBookCreationRequest request);

        public Task<ApiResponse<HeaderBookUpdateResponse>> UpdateBookAsync(HeaderBookUpdateRequest request, string idBook);
        
        public Task<ApiResponse<string>> DeleteBookAsync(string idBook);
        
        public Task<ApiResponse<ChangeStatusOfTheBookResponse>> ChangeStatusOfTheBookAsync(ChangeStatusOfTheBookRequest request);

        public Task<ApiResponse<List<EvaluationDetails>>> GetBooksEvaluation(EvaluationRequest evaluationRequest);
        
        public Task<ApiResponse<bool>> LikeBook(EvaluationRequest evaluationRequest);

        public Task<ApiResponse<List<BooksAndComments>>> GetFavoriteBook(string idReader);

        public Task<ApiResponse<bool>> DeleteEvaluation(DeleteEvaluationRequest deleteEvaluationRequest);

        public Task<ApiResponse<List<GetHeaderbookResponse>>> GetAllHeaderBooks();

        public Task<ApiResponse<List<GetHeaderbookResponse>>> GetAllHeaderBooksByTheBook(string idThebook);

        public Task<ApiResponse<List<BooksAndComments>>> GetAllBooksInDetail(string idReader);

        public Task<ApiResponse<List<BooksAndCommentsResponse>>> FindBook(string namebook);

        public Task<ApiResponse<List<BooksAndCommentsResponse>>> GetAllBooksInDetailById(string idbook);

        public Task<ApiResponse<bool>> AddEvaluation(string idreader, AddEvaluationRequest addEvaluationRequest);

        public Task<ApiResponse<List<CommentResponse>>> GetAllCommentByIdBook(string idbook);

        public Task<ApiResponse<List<EvaResponse>>> GetAllStar(string idbook);

        public Task<ApiResponse<bool>> EditCommentAsync(string idComment, string response, int rate);

        public Task<ApiResponse<bool>> DeleteComment(string idComment, string idReader);

        public Task<ApiResponse<List<TheBookStatusResponse>>> GetTheBookStatus(string idThebook);
    } 
}
