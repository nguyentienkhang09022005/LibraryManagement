using LibraryManagement.Dto.Request;
using LibraryManagement.Service;
using LibraryManagement.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OverdueReportController : ControllerBase
    {
        private readonly IOverdueReportService _overdueReportService;

        public OverdueReportController(IOverdueReportService overdueReportService)
        {
            _overdueReportService = overdueReportService;
        }

        // Endpoint tạo báo cáo sách trả trễ theo ngày
        [Authorize]
        [HttpPost("add-overdue-report")]
        public async Task<IActionResult> addOverdueReport([FromBody] OverdueReportRequest request)
        {
            var result = await _overdueReportService.addOverdueReportAsync(request);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        // Endpoint xóa báo cáo sách trả trễ
        [Authorize]
        [HttpDelete("delete-overdue-report")]
        public async Task<IActionResult> deleteOverdueReport([FromQuery] Guid idOverdueReport)
        {
            var result = await _overdueReportService.deleteOverReportAsync(idOverdueReport);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }
    }
}
