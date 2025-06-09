using LibraryManagement.Data;
using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers;
using LibraryManagement.Models;
using LibraryManagement.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Service
{
    public class CategoryReportService : ICategoryReportService
    {
        private readonly LibraryManagermentContext _context;

        public CategoryReportService(LibraryManagermentContext context)
        {
            _context = context;
        }

        // Tạo báo cáo thể loại theo tháng
        public async Task<ApiResponse<CategoryReportResponse>> addCategoryReportAsync(CategoryReportRequest request)
        {
            var month = request.MonthReport;
            var year = DateTime.Now.Year;

            // Lấy tất cả lượt mượn trong tháng kèm theo thông tin TypeBook
            var loanSlipsInMonth = await _context.LoanSlipBooks
                .Where(l => l.BorrowDate.Month == month && l.BorrowDate.Year == year)
                .Include(l => l.TheBook)
                    .ThenInclude(tb => tb.Book)
                        .ThenInclude(b => b.HeaderBook)
                            .ThenInclude(hb => hb.TypeBook)
                .ToListAsync();

            // Đếm số lượt mượn theo IdTypeBook
            var borrowCounts = loanSlipsInMonth
                .GroupBy(l => l.TheBook.Book.HeaderBook.IdTypeBook)
                .Select(countTypeBookById => new
                {
                    IdTypeBook = countTypeBookById.Key,
                    BorrowCount = countTypeBookById.Count()
                })
                .ToList();

            int totalBorrowCount = borrowCounts.Sum(total => total.BorrowCount);

            // Kiểm tra nếu báo cáo đã tồn tại thì cập nhật
            var categoryReport = await _context.CategoryReports
                .FirstOrDefaultAsync(r => r.MonthReport == month && r.YearReport == year);

            if (categoryReport != null)
            {
                // Cập nhật tổng lượt mượn
                categoryReport.TotalBorrowCount = totalBorrowCount;

                // Xóa chi tiết cũ
                var oldDetails = await _context.CategoryReportDetails
                    .Where(d => d.IdCategoryReport == categoryReport.IdCategoryReport)
                    .ToListAsync();
                _context.CategoryReportDetails.RemoveRange(oldDetails);
            }else
            {
                // Nếu chưa có thì tạo mới
                categoryReport = new CategoryReport
                {
                    MonthReport = month,
                    YearReport = year,
                    TotalBorrowCount = totalBorrowCount
                };
                _context.CategoryReports.Add(categoryReport);
                await _context.SaveChangesAsync();
            }

            foreach (var item in borrowCounts)
            {
                var detail = new CategoryReportDetail
                {
                    IdCategoryReport = categoryReport.IdCategoryReport,
                    IdTypeBook = item.IdTypeBook,
                    BorrowCount = item.BorrowCount,
                    BorrowRatio = totalBorrowCount == 0 ? 0 : (float)item.BorrowCount / totalBorrowCount * 100
                };
                _context.CategoryReportDetails.Add(detail);
            }
            await _context.SaveChangesAsync();

            var reportDetails = await _context.CategoryReportDetails
                .Where(d => d.IdCategoryReport == categoryReport.IdCategoryReport)
                .Include(d => d.TypeBook)
                .ToListAsync();

            var detailResponses = reportDetails.Select(detail => new CategoryDetailReportResponse
            {
                IdCategoryReport = detail.IdCategoryReport,
                BorrowCount = detail.BorrowCount,
                BorrowRatio = detail.BorrowRatio,
                typeBookResponse = new TypeBookResponse
                {
                    IdTypeBook = detail.TypeBook.IdTypeBook,
                    NameTypeBook = detail.TypeBook.NameTypeBook
                }
            }).ToList();

            return ApiResponse<CategoryReportResponse>.SuccessResponse($"Đã tạo báo cáo tháng {month} thành công", 200, new CategoryReportResponse
            {
                IdCategoryReport = categoryReport.IdCategoryReport,
                MonthReport = categoryReport.MonthReport,
                TotalBorrowCount = categoryReport.TotalBorrowCount,
                categoryDetailReportResponse = detailResponses
            });
        }

        // Xóa báo cáo
        public async Task<ApiResponse<string>> deleteCategoryReportAsync(Guid idCategoryReport)
        {
            var categoryReport = await _context.CategoryReports.FirstOrDefaultAsync(report => report.IdCategoryReport == idCategoryReport);
            if (categoryReport == null)
            {
                return ApiResponse<string>.FailResponse("Không tìm thấy báo cáo", 404);
            }
            _context.CategoryReports.Remove(categoryReport);
            await _context.SaveChangesAsync();
            return ApiResponse<string>.SuccessResponse("Đã xóa báo cáo thành công", 200, "");
        }

        public async Task<List<CategoryOverdueResponse>> getOverdueReport()
        {
            var result = await (from obd in _context.OverdueReportDetails.AsNoTracking()
                                .Include(d => d.OverdueReport)
                                .Include(d => d.TheBook)
                                    .ThenInclude(t => t.Book)
                                        .ThenInclude(b => b.HeaderBook)
                                join lsb in _context.LoanSlipBooks
                                    on new { obd.IdTheBook, obd.BorrowDate } equals new { lsb.IdTheBook, lsb.BorrowDate }
                                join r in _context.Readers
                                    on lsb.IdReader equals r.IdReader
                                select new CategoryOverdueResponse
                                {
                                    IdOverDueReport = obd.OverdueReport.IdOverdueReport,
                                    reportDate = obd.OverdueReport.CreatedDate,
                                    IDbook = obd.IdTheBook,
                                    BookName = obd.TheBook.Book.HeaderBook.NameHeaderBook,
                                    DateBorrow = obd.BorrowDate,
                                    DateLate = obd.LateDays, 
                                    IDuser = lsb.IdReader,
                                    username = r.NameReader!,
                                    totalfine = lsb.FineAmount
                                })
                            .OrderByDescending(x => x.reportDate)
                            .ThenByDescending(x => x.DateLate)
                            .ToListAsync();
            return result; 
        }

        // Sửa thông tin báo cáo
        public async Task<ApiResponse<CategoryReportResponse>> updateCategoryReportAsync(CategoryReportRequest request, Guid idCategoryReport)
        {
            throw new NotImplementedException();
        }
    }
}
