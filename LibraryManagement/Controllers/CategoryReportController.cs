using LibraryManagement.Dto.Request;
using LibraryManagement.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZstdSharp.Unsafe;

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryReportController : ControllerBase
    {
        private readonly ICategoryReportService _categoryReportService;
        public CategoryReportController(ICategoryReportService categoryReportService)
        {
            _categoryReportService = categoryReportService;
        }

        // Endpoint tạo báo cáo thể loại theo tháng
        [HttpPost("add_category_report")]
        public async Task<IActionResult> addCategoryReport([FromBody] CategoryReportRequest request)
        {
            var result = await _categoryReportService.addCategoryReportAsync(request);
            if (result.Success)
                return Created("", result);
            return BadRequest(result);
        }

        // Endpoint xóa báo cáo thể loại theo tháng
        [HttpDelete("delete_category_report/{idCategoryReport}")]
        public async Task<IActionResult> deleteCategoryReport(Guid idCategoryReport)
        {
            var result = await _categoryReportService.deleteCategoryReportAsync(idCategoryReport);
            if (result.Success)
                return Ok(result);
            return NotFound(result);
        }
        [HttpGet("getCategoryOverdueReport")]
           [Authorize(Policy = "JwtOrCookie")]
        public async Task<IActionResult> GetCategoryOverdueReport()
        {
            try
            {
                var result = await _categoryReportService.getOverdueReport();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            }

    }
}
