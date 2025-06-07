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
        public Task<ApiResponse<AuthorResponse>> updateAuthorAsync(AuthorRequest request, Guid idAuthor);
        public Task<ApiResponse<string>> deleteAuthorAsync(Guid idAuthor);

        public Task<List<Author>> findAuthor(FindAuthorInputDto dto);
    }
}
