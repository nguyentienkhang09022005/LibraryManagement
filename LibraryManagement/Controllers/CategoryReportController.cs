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

        [Authorize]
        [HttpPost("add-category-report")]
        public async Task<IActionResult> addCategoryReport([FromBody] CategoryReportRequest request)
        {
            var result = await _categoryReportService.addCategoryReportAsync(request);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpDelete("delete-category-report")]
        public async Task<IActionResult> deleteCategoryReport([FromQuery] Guid idCategoryReport)
        {
            var result = await _categoryReportService.deleteCategoryReportAsync(idCategoryReport);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpGet("get-category-overdue-report")]
        public async Task<IActionResult> GetCategoryOverdueReport()
        {
            var result = await _categoryReportService.getOverdueReport();
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }
    }
}
