using LibraryManagement.Data;
using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers;
using LibraryManagement.Models;
using LibraryManagement.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Service
{
    public class OverdueReportService : IOverdueReportService
    {
        private readonly LibraryManagermentContext _context;    

        public OverdueReportService(LibraryManagermentContext context)
        {
            _context = context;
        }

        // Tạo báo cáo trả trễ theo ngày
        public async Task<ApiResponse<OverdueReportResponse>> addOverdueReportAsync(OverdueReportRequest request)
        {
            var createdDateUtc = DateTime.SpecifyKind(request.CreatedDate.Date, DateTimeKind.Utc);

            var overdueReport = await _context.OverdueReports
                .FirstOrDefaultAsync(or => or.CreatedDate == createdDateUtc);
            if (overdueReport == null)
            {
                overdueReport = new OverdueReport()
                {
                    CreatedDate = DateTime.SpecifyKind(request.CreatedDate, DateTimeKind.Utc),
                    OverdueReportDetails = new List<OverdueReportDetail>()
                };
                _context.OverdueReports.Add(overdueReport);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Xóa chi tiết báo cáo nếu đã tồn tại
                if (overdueReport.OverdueReportDetails != null && overdueReport.OverdueReportDetails.Any())
                {
                    _context.OverdueReportDetails.RemoveRange(overdueReport.OverdueReportDetails);
                    await _context.SaveChangesAsync();
                }
            }

            // Lấy danh sách sách trả trễ trong ngày
            var nextDateUtc = createdDateUtc.AddDays(1);

            var overdueBooks = await _context.LoanSlipBooks
                .Where(ob => ob.ReturnDate >= createdDateUtc && ob.ReturnDate < nextDateUtc)
                .ToListAsync();

            var overdueBooksFiltered = overdueBooks
                .Where(ob => (ob.ReturnDate - ob.BorrowDate).TotalDays > ob.LoanPeriod)
                .ToList();

            var bookIds = overdueBooksFiltered.Select(b => b.IdTheBook).ToList();

            var bookInfoMap = await _context.TheBooks
                .Include(tb => tb.Book)
                    .ThenInclude(b => b.HeaderBook)
                .Where(tb => bookIds.Contains(tb.IdTheBook))
                .ToDictionaryAsync(
                    tb => tb.IdTheBook,
                    tb => tb.Book.HeaderBook.NameHeaderBook
                );

            // Tạo danh sách chi tiết mới
            var newDetails = overdueBooks.Select(book => new OverdueReportDetail
            {
                IdOverdueReport = overdueReport.IdOverdueReport,
                IdTheBook = book.IdTheBook,
                BorrowDate = DateTime.SpecifyKind(book.BorrowDate, DateTimeKind.Utc),
                LateDays = Math.Abs((book.ReturnDate - book.BorrowDate).Days - book.LoanPeriod)
            }).ToList();

            if (newDetails.Any())
            {
                _context.OverdueReportDetails.AddRange(newDetails);
                await _context.SaveChangesAsync();
            }

            // Tạo response
            var response = new OverdueReportResponse
            {
                IdOverdueReport = overdueReport.IdOverdueReport,
                CreatedDate = overdueReport.CreatedDate,
                Detail = newDetails.Select(d => new OverdueReportDetailResponse
                {
                    IdOverdueReport = d.IdOverdueReport,
                    IdTheBook = d.IdTheBook,
                    NameHeaderBook = bookInfoMap.ContainsKey(d.IdTheBook) ? bookInfoMap[d.IdTheBook] : "Không rõ",
                    BorrowDate = d.BorrowDate,
                    LateDays = d.LateDays
                }).ToList()
            };

            return ApiResponse<OverdueReportResponse>.SuccessResponse("Tạo báo cáo trả trễ thành công", 200, response);
        }

        // Xóa báo cáo
        public async Task<ApiResponse<string>> deleteOverReportAsync(Guid idOverReport)
        {
            var overdueReport = await _context.OverdueReports.FirstOrDefaultAsync(report => report.IdOverdueReport == idOverReport);
            if (overdueReport == null)
            {
                return ApiResponse<string>.FailResponse("Không tìm thấy báo cáo", 404);
            }
            _context.OverdueReports.Remove(overdueReport);
            await _context.SaveChangesAsync();
            return ApiResponse<string>.SuccessResponse("Đã xóa báo cáo thành công", 200, "");
        }



        public Task<ApiResponse<OverdueReportResponse>> updateOverdueReportAsync(OverdueReportRequest request, Guid idOverReport)
        {
            throw new NotImplementedException();
        }
    }
}
