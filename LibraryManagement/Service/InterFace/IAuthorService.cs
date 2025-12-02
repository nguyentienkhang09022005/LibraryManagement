using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers;

namespace LibraryManagement.Repository.InterFace
{
    public interface IAuthorService
    {
        Task<ApiResponse<List<AuthorResponse>>> GetListAuthor();

        Task<ApiResponse<AuthorResponse>> AddAuthorAsync(AuthorCreationRequest request);

        Task<ApiResponse<AuthorResponse>> UpdateAuthorAsync(AuthorUpdateRequest request, Guid idAuthor);

        Task<ApiResponse<string>> DeleteAuthorAsync(Guid idAuthor);

        Task<ApiResponse<List<AuthorResponse>>> FindAuthor(AuthorFindNameRequest authorFindNameRequest);

        Task<ApiResponse<GetAuthorByIdResponse>> GetAuthorById(Guid idauthor);
    }
}
