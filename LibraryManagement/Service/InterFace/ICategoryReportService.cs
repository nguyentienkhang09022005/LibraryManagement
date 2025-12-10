using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers;

namespace LibraryManagement.Service.Interface
{
    public interface ICategoryReportService
    {
        public Task<ApiResponse<CategoryReportResponse>> addCategoryReportAsync(CategoryReportRequest request);

        public Task<ApiResponse<string>> deleteCategoryReportAsync(Guid idCategoryReport);

        Task<ApiResponse<List<CategoryOverdueResponse>>> getOverdueReport();
    }
}
