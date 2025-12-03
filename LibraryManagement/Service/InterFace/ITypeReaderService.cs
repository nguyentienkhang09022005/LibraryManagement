using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers;

namespace LibraryManagement.Repository.InterFace
{
    public interface ITypeReaderService
    {
        Task<ApiResponse<TypeReaderResponse>> AddTypeReaderAsync(TypeReaderRequest request);

        Task<ApiResponse<TypeReaderResponse>> UpdateTypeReaderAsync(TypeReaderRequest request, Guid idTypeReader);
        
        Task<ApiResponse<string>> DeleteTypeReaderAsync(Guid idTypeReader);

        Task<ApiResponse<List<TypeReaderResponse>>> GetAllTypeReader();
    }
}
