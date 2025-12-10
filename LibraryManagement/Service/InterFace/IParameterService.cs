
using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers;
using LibraryManagement.Models;


namespace LibraryManagement.Repository.InterFace
{
    public interface IParameterService
    {
        Task<ApiResponse<ParameterResponse>> addParameterAsync(ParameterRequest request);

        Task<ApiResponse<ParameterResponse>> updateParameterAsync(ParameterRequest request, Guid idParameter);

        Task<ApiResponse<string>> deleteParameterAsync(Guid idParameter);

        Task<int> getValueAsync(string nameParameter);

        public Task<ApiResponse<List<Parameter>>> getParametersAsync();
    }
}
