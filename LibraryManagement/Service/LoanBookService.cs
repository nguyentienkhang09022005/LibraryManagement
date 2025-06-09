using LibraryManagement.Data;
using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers;
using LibraryManagement.Models;
using LibraryManagement.Repository.InterFace;
using LibraryManagement.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Repository
{
    public class LoanBookService : ILoanBookService
    {
        private readonly LibraryManagermentContext _context;
        private readonly IAuthenService _account;
        private readonly IParameterService _parameterRepository;


        public LoanBookService(LibraryManagermentContext context, 
                                      IAuthenService authen, 
                                      IParameterService parameterRepository)
        {
            _account = authen;
            _context = context;
            _parameterRepository = parameterRepository;
        }

        // Tạo phiếu mượn sách
        public async Task<ApiResponse<LoanBookResponse>> addLoanBookAsync(LoanBookRequest request)
        {
            // Các quy định
            int cardExpirationMonths = await _parameterRepository.getValueAsync("CardExpirationDate");
            int borrowingLimit = await _parameterRepository.getValueAsync("BorrowingLimit");
            int borrowingPeriodDays = await _parameterRepository.getValueAsync("BorrowingPeriodDays");

            // Kiểm tra Reader có tồn tại hay không
            var reader = await _context.Readers.FirstOrDefaultAsync(rd => rd.IdReader == request.IdReader);
            if (reader == null)
            {
                return ApiResponse<LoanBookResponse>.FailResponse("không tìm thấy độc giả", 404);
            }

            // Kiểm tra TheBook có tồn tại hay không
            var theBook = await _context.TheBooks.FirstOrDefaultAsync(tb => tb.IdTheBook == request.IdTheBook);
            if (theBook == null)
            {
                return ApiResponse<LoanBookResponse>.FailResponse("không tìm thấy cuốn sách", 404);
            }

            // Kiểm tra trạng thái cuốn sách
            if (theBook.Status == "Đã mượn")
            {
                return ApiResponse<LoanBookResponse>.FailResponse("Cuốn sách đang được mượn", 400);
            }

            // Kiểm tra thẻ độc giả còn hạn hay không
            DateTime cardIssueDate = reader.CreateDate;
            DateTime cardExpirationDate = cardIssueDate.AddMonths(cardExpirationMonths);
            if (cardExpirationDate < DateTime.UtcNow)
            {
                return ApiResponse<LoanBookResponse>.FailResponse("Thẻ độc giả đã quá hạn 6 tháng", 400);
            }

            // Kiểm tra trong 4 ngày không được mượn quá 5 quyển sách
            DateTime dateThreshold = DateTime.UtcNow.AddDays(-borrowingPeriodDays);
            int borrowedInPeriod = await _context.LoanSlipBooks
                .Where(l => l.IdReader == request.IdReader && l.BorrowDate >= dateThreshold)
                .CountAsync();


            if (borrowedInPeriod >= borrowingLimit)
            {
                return ApiResponse<LoanBookResponse>.FailResponse($"Trong {borrowingPeriodDays} ngày gần nhất, độc giả đã mượn tối đa {borrowingLimit} cuốn sách", 400);
            }

            DateTime borrowDate = DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Utc);
            DateTime returnDate = borrowDate.AddDays(borrowingPeriodDays);

            var loanBook = new LoanSlipBook
            {
                IdTheBook = request.IdTheBook,
                IdReader = request.IdReader,
                BorrowDate = borrowDate,
                ReturnDate = returnDate
            };

            _context.LoanSlipBooks.Add(loanBook);
            theBook.Status = "Đã mượn";
            _context.TheBooks.Update(theBook);

            await _context.SaveChangesAsync();

            // Lấy TheBook cùng Book, HeaderBook, TypeBook
            var theBookWithDetails = await _context.TheBooks
                .Include(thebook => thebook.Book)
                    .ThenInclude(book => book.HeaderBook)
                        .ThenInclude(headerbook => headerbook.TypeBook)
                .FirstOrDefaultAsync(thebook => thebook.IdTheBook == request.IdTheBook);

            if (theBookWithDetails == null)
            {
                return ApiResponse<LoanBookResponse>.FailResponse("Không tìm thấy thông tin chi tiết của cuốn sách", 404);
            }

            // Lấy HeaderBook
            var headerBook = theBookWithDetails.Book.HeaderBook;

            // Lấy danh sách tác giả
            var authors = await _context.BookWritings
                .Where(bw => bw.IdHeaderBook == headerBook.IdHeaderBook)
                .Select(bw => new AuthorInLoanBookResponse
                {
                    IdAuthor = bw.Author.IdAuthor,
                    NameAuthor = bw.Author.NameAuthor
                })
                .ToListAsync();

            var response = new LoanBookResponse
            {
                IdLoanSlipBook = loanBook.IdLoanSlipBook,
                BookResponse = new TheBookInLoanBookResponse
                {
                    IdTheBook = theBookWithDetails.IdTheBook,
                    NameHeaderBook = headerBook.NameHeaderBook,
                    TypeBook = new TypeBookResponse
                    {
                        IdTypeBook = headerBook.TypeBook.IdTypeBook,
                        NameTypeBook = headerBook.TypeBook.NameTypeBook
                    },
                    Authors = authors,
                },
                readerResponse = new ReaderInLoanBookResponse
                {
                    IdReader = reader.IdReader,
                    NameReader = reader.NameReader!
                },
                BorrowDate = loanBook.BorrowDate,
                ReturnDate = loanBook.ReturnDate
            };
            return ApiResponse<LoanBookResponse>.SuccessResponse("Tạo phiếu mượn thành công", 200, response);
        }

        // Xóa phiếu mượn
        public async Task<ApiResponse<string>> deleteLoanBookAsync(Guid idLoanSlipBook)
        {
            var deleteLoanBook = await _context.LoanSlipBooks.FirstOrDefaultAsync(lb => lb.IdLoanSlipBook == idLoanSlipBook);
            if (deleteLoanBook == null)
            {
                return ApiResponse<string>.FailResponse("Không tìm thấy phiếu mượn", 404);
            }
            var theBook = await _context.TheBooks
                .FirstOrDefaultAsync(tb => tb.IdTheBook == deleteLoanBook.IdTheBook);

            if (theBook != null)
            {
                theBook.Status = "Có sẵn";
                _context.TheBooks.Update(theBook);
            }

            _context.LoanSlipBooks.Remove(deleteLoanBook);
            await _context.SaveChangesAsync();
            return ApiResponse<string>.SuccessResponse("Đã xóa phiếu mượn sách thành công", 200, "");
        }

        // Sửa phiếu mượn sách


        public async Task<List<LoanSlipBookResponse>> getListLoanSlipBook(string token)
        {
            
            var result = await _context.LoanSlipBooks.Select(a => new LoanSlipBookResponse
            {
                IdLoanSlipBook = a.IdLoanSlipBook,
                IdTheBook = a.IdTheBook,
                IdReader = a.IdReader,
                BorrowDate = a.BorrowDate,
                ReturnDate = a.ReturnDate,
                FineAmount = a.FineAmount
            }).ToListAsync();
            return result; 
        }

        public async Task<List<GetLoanSlipBookByType>> getLoanSlipBookByType(string? genre)
        {
            if (genre != null)
            {
                 return await _context.CategoryReportDetails
                    .Include(x => x.CategoryReport)
                    .Include(x => x.TypeBook)
                    .Where(x => x.TypeBook.NameTypeBook.ToLower().Contains(genre.ToLower()))
                    .Select(x => new GetLoanSlipBookByType
                    {
                        IdReportGenre = x.IdCategoryReport,
                        ReportMonth = x.CategoryReport.MonthReport,
                        Genre = x.TypeBook.NameTypeBook,
                        TotalBorrow = x.BorrowCount,
                        Rate = x.BorrowRatio
                    })
                     .OrderByDescending(x => x.ReportMonth)
                     .ThenByDescending(x => x.TotalBorrow)
                    .ToListAsync();
            }
            else
            {
                 return  await _context.CategoryReportDetails
                   .Include(x => x.CategoryReport)
                   .Include(x => x.TypeBook)
                   .Select(x => new GetLoanSlipBookByType
                   {
                       IdReportGenre = x.IdCategoryReport,
                       ReportMonth = x.CategoryReport.MonthReport,
                       Genre = x.TypeBook.NameTypeBook,
                       TotalBorrow = x.BorrowCount,
                       Rate = x.BorrowRatio
                   })
                    .OrderByDescending(x => x.ReportMonth)
                    .ThenByDescending(x => x.TotalBorrow)
                   .ToListAsync();
            }
              
        }

        public async Task<List<LoanBookHistory>> getLoanSlipBookByUser(string idReader)
        {
            var result = await _context.LoanSlipBooks
                .Include(x => x.Reader)
                .Where(x => x.IdReader == idReader)
                .Select(x => new LoanBookHistory
                {
                    IdBook = x.TheBook.IdBook,
                    NameBook = x.TheBook.Book.HeaderBook.NameHeaderBook,
                    Genre = x.TheBook.Book.HeaderBook.TypeBook.NameTypeBook,
                    DateBorrow = x.BorrowDate,
                    DateReturn = x.ReturnDate,
                    AvatarUrl = (x.TheBook.Book.images.FirstOrDefault() == null) ? string.Empty : x.TheBook.Book.images.FirstOrDefault()!.Url
                }).ToListAsync();
            return result;
        }
    }
}
