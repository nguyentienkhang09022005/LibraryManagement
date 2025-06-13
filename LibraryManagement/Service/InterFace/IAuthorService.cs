using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers;
using LibraryManagement.Models;

namespace LibraryManagement.Repository.InterFace
{
    public interface IAuthorService
    {
        public Task<List<AuthorResponse>> getListAuthor();
        public Task<ApiResponse<AuthorResponse>> addAuthorAsync(AuthorRequest request);
        public Task<ApiResponse<AuthorResponse>> updateAuthorAsync(AuthorUpdateRequest request, Guid idAuthor);
        public Task<ApiResponse<string>> deleteAuthorAsync(Guid idAuthor);

        public Task<List<AuthorResponse>> findAuthor(FindAuthorInputDto dto);

        public Task<GetAuthorByIdResponse> GetAuthorById(Guid idauthor);
    }
}
