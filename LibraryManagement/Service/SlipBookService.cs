using LibraryManagement.Data;
using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers;
using LibraryManagement.Repository.InterFace;
using LibraryManagement.Service.InterFace;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Service
{
    public class SlipBookService : ISlipBookService
    {
        private readonly LibraryManagermentContext _context;
        private readonly IParameterService _parameterRepository;


        public SlipBookService(LibraryManagermentContext context,
                                      IParameterService parameterRepository)
        {
            _context = context;
            _parameterRepository = parameterRepository;
        }

        // Tạo phiếu trả sách
        public async Task<ApiResponse<SlipBookResponse>> addSlipBookAsync(SlipBookRequest request)
        {
            int finePerOverdueDay = await _parameterRepository.getValueAsync("FinePerOverdueDay");

            var loanbook = await _context.LoanSlipBooks.FirstOrDefaultAsync(loan => loan.IdLoanSlipBook == request.IdLoanSlipBook);

            if (loanbook == null)
            {
                return ApiResponse<SlipBookResponse>.FailResponse("Không tìm thấy phiếu mượn!", 404);
            }

            if (loanbook.IsReturned)
            {
                return ApiResponse<SlipBookResponse>.FailResponse("Phiếu mượn này đã được trả trước đó!", 400);
            }

            DateTime borrowDate = DateTime.SpecifyKind(loanbook.BorrowDate, DateTimeKind.Utc);
            DateTime scheduledReturnDate = DateTime.SpecifyKind(loanbook.ReturnDate, DateTimeKind.Utc);
            DateTime actualReturnDate = DateTime.UtcNow;

            int overdueDays = (actualReturnDate.Date - scheduledReturnDate.Date).Days;
            overdueDays = Math.Max(0, overdueDays);

            decimal fineAmount = overdueDays * finePerOverdueDay;

            // Cập nhật trạng thái cuốn sách
            var theBook = await _context.TheBooks.FirstOrDefaultAsync(tb => tb.IdTheBook == request.IdTheBook);
            if (theBook != null)
            {
                theBook.Status = "Có sẵn";
                _context.TheBooks.Update(theBook);
            }

            // Cập nhật tổng nợ của độc giả
            var reader = await _context.Readers.FirstOrDefaultAsync(r => r.IdReader == request.IdReader);
            if (reader == null)
            {
                return ApiResponse<SlipBookResponse>.FailResponse("Không tìm thấy độc giả!", 404);
            }
            reader.TotalDebt += fineAmount;
            _context.Readers.Attach(reader);
            _context.Entry(reader).Property(r => r.TotalDebt).IsModified = true;

            // Cập nhật bảng LoanSlipBook
            loanbook.LoanPeriod = (actualReturnDate.Date - borrowDate.Date).Days;
            loanbook.FineAmount = fineAmount;
            loanbook.IsReturned = true;
            _context.LoanSlipBooks.Attach(loanbook);
            _context.Entry(loanbook).Property(l => l.LoanPeriod).IsModified = true;
            _context.Entry(loanbook).Property(l => l.FineAmount).IsModified = true;

            await _context.SaveChangesAsync();

            var response = new SlipBookResponse
            {
                IdLoanSlipBook = loanbook.IdLoanSlipBook,
                IdTheBook = request.IdTheBook,
                BorrowDate = borrowDate,
                ReturnDate = scheduledReturnDate,
                LoanPeriod = (actualReturnDate.Date - borrowDate.Date).Days,
                FineAmount = fineAmount,
                readerResponse = new ReaderInSlipBookResponse
                {
                    IdReader = reader.IdReader,
                    NameReader = reader.NameReader,
                    TotalDebt = reader.TotalDebt
                }
            };

            return ApiResponse<SlipBookResponse>.SuccessResponse("Tạo phiếu trả sách thành công!", 200, response);
        }

        // Xóa phiếu trả sách
        public async Task<ApiResponse<string>> deleteSlipBookAsync(Guid idLoanSlipBook)
        {
            var deleteSlipBook = await _context.LoanSlipBooks.FirstOrDefaultAsync(lb => lb.IdLoanSlipBook == idLoanSlipBook);
            if (deleteSlipBook == null)
            {
                return ApiResponse<string>.FailResponse("Không tìm thấy phiếu trả!", 404);
            }

            // Nếu đã có ngày trả thì mới thực hiện cập nhật sách và nợ
            bool hasReturned = deleteSlipBook.ReturnDate != null;

            if (hasReturned)
            {
                var theBook = await _context.TheBooks
                    .FirstOrDefaultAsync(tb => tb.IdTheBook == deleteSlipBook.IdTheBook);
                if (theBook != null)
                {
                    theBook.Status = "Đã mượn";
                    _context.TheBooks.Update(theBook);
                }

                var reader = await _context.Readers
                    .FirstOrDefaultAsync(rd => rd.IdReader == deleteSlipBook.IdReader);
                if (reader != null)
                {
                    reader.TotalDebt -= deleteSlipBook.FineAmount;
                    reader.TotalDebt = Math.Max(reader.TotalDebt, 0);
                    _context.Readers.Attach(reader);
                    _context.Entry(reader).Property(r => r.TotalDebt).IsModified = true;
                    await _context.SaveChangesAsync();
                }
            }
            _context.LoanSlipBooks.Remove(deleteSlipBook);
            await _context.SaveChangesAsync();
            return ApiResponse<string>.SuccessResponse("Đã xóa phiếu trả sách thành công!", 200, string.Empty);
        }
    }
}
