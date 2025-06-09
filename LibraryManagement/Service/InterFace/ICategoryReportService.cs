using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers;

namespace LibraryManagement.Service.Interface
{
    public interface ICategoryReportService
    {
        public Task<ApiResponse<CategoryReportResponse>> addCategoryReportAsync(CategoryReportRequest request);
        public Task<ApiResponse<CategoryReportResponse>> updateCategoryReportAsync(CategoryReportRequest request, Guid idCategoryReport);
        public Task<ApiResponse<string>> deleteCategoryReportAsync(Guid idCategoryReport);

        public Task<List<CategoryOverdueResponse>> getOverdueReport();
    }
}
