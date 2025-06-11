using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers;

namespace LibraryManagement.Repository.InterFace
{
    public interface ITypeReaderService
    {
        public Task<ApiResponse<TypeReaderResponse>> addTypeReaderAsync(TypeReaderRequest request);
        public Task<ApiResponse<TypeReaderResponse>> updateTypeReaderAsync(TypeReaderRequest request, Guid idTypeReader);
        public Task<ApiResponse<string>> deleteTypeReaderAsync(Guid idTypeReader);
     
    }
}
