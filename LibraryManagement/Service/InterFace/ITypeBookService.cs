using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers;

namespace LibraryManagement.Repository.InterFace
{
    public interface ITypeBookService
    {
        Task<ApiResponse<TypeBookResponse>> AddTypeBookAsync(TypeBookRequest request);

        Task<ApiResponse<TypeBookResponse>> UpdateTypeBookAsync(TypeBookRequest request, Guid idTypeBook);

        Task<ApiResponse<string>> DeleteTypeBook(Guid idTypeBook);

        Task<ApiResponse<List<TypeBookResponseAndBook>>> GetTypebookAndHeaders();

        Task<ApiResponse<List<TypeBookResponse>>> GetAllTypeBook();
    }
}
