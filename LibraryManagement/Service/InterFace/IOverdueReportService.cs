using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers;

namespace LibraryManagement.Service.Interface
{
    public interface IOverdueReportService
    {
        public Task<ApiResponse<OverdueReportResponse>> addOverdueReportAsync(OverdueReportRequest request);
        public Task<ApiResponse<OverdueReportResponse>> updateOverdueReportAsync(OverdueReportRequest request, Guid idOverReport);
        public Task<ApiResponse<string>> deleteOverReportAsync(Guid idOverReport);

    }
}
