using LibraryManagement.Data;
using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers;
using LibraryManagement.Models;
using LibraryManagement.Repository.InterFace;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Repository
{
    public class PenaltyTicketService : IPenaltyTicketService
    {
        private readonly LibraryManagermentContext _context;
        private readonly IParameterService _parameterService;

        public PenaltyTicketService(LibraryManagermentContext context, IParameterService parameterService)
        {
            _context = context;
            _parameterService = parameterService;
        }

        // Tạo phiếu thu tiền phạt
        public async Task<ApiResponse<PenaltyTicketResponse>> addPenaltyTicketAsync(PenaltyTicketRequest request)
        {
            var reader = await _context.Readers.FirstOrDefaultAsync(r => r.IdReader == request.IdReader);
            if (reader == null)
            {
                return ApiResponse<PenaltyTicketResponse>.FailResponse("Không tìm thấy độc giả!", 404);
            }

            // Áp dụng quy định kiểm tra số tiền thu 
            int policyValue = await _parameterService.getValueAsync("LateReturnPenaltyPolicy");
            if (policyValue == 1 && request.AmountCollected > reader.TotalDebt)
            {
                return ApiResponse<PenaltyTicketResponse>.FailResponse("Số tiền thu vượt quá số tiền độc giả đang nợ!", 400);
            }

            // Tạo phiếu thu
            var penalty = new PenaltyTicket
            {
                IdReader = request.IdReader,
                CreatedDate = DateTime.UtcNow,
                AmountCollected = request.AmountCollected,
                AmountRemaining = reader.TotalDebt - request.AmountCollected
            };

            reader.TotalDebt = penalty.AmountRemaining;

            _context.PenaltyTickets.Add(penalty);
            _context.Readers.Attach(reader);
            _context.Entry(reader).Property(r => r.TotalDebt).IsModified = true;
            await _context.SaveChangesAsync();

            return ApiResponse<PenaltyTicketResponse>.SuccessResponse("Tạo phiếu thu tiền phạt thành công!", 200, new PenaltyTicketResponse
            {
                IdPenalty = penalty.IdPenalty,
                CreatedDate = penalty.CreatedDate,
                AmountCollected = penalty.AmountCollected,
                AmountRemaining = penalty.AmountRemaining,
                ReaderResponse = new ReaderInPenaltyTicket
                {
                    IdReader = reader.IdReader,
                    NameReader = reader.NameReader
                }
            });
        }

        // Xóa phiếu thu tiền phạt
        public async Task<ApiResponse<string>> deletePenaltyTicketAsync(Guid idPenaltyTicket)
        {
            var penalty = await _context.PenaltyTickets.Include(p => p.Reader).FirstOrDefaultAsync(p => p.IdPenalty == idPenaltyTicket);
            if (penalty == null)
                return ApiResponse<string>.FailResponse("Không tìm thấy phiếu thu!", 404);

            var reader = penalty.Reader;
            reader.TotalDebt += penalty.AmountCollected;

            _context.Readers.Attach(reader);
            _context.Entry(reader).Property(r => r.TotalDebt).IsModified = true;

            _context.PenaltyTickets.Remove(penalty);
            await _context.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse("Xóa phiếu thu thành công!", 200, string.Empty);
        }

        public async Task<ApiResponse<List<TicketResponse>>> GetTicketResponsesAsync(string idUser)
        {
            var result = await _context.PenaltyTickets.AsNoTracking()
                .Where(x => x.IdReader == idUser)
                .Select(x => new TicketResponse
                {
                    CreatedDate = x.CreatedDate,
                    AmountCollected = x.AmountCollected,
                    AmountRemaining = x.AmountRemaining,
                    TotalDebit = x.Reader.TotalDebt,
                })
                .ToListAsync();
            return ApiResponse<List<TicketResponse>>.SuccessResponse("Lấy danh sách phiếu thu tiền phạt thành công!", 200, result);
        }

        // Sửa phiếu thu tiền phạt
        public async Task<ApiResponse<PenaltyTicketResponse>> updatePenaltyTicketAsync(PenaltyTicketRequest request, Guid idPenaltyTicket)
        {
            var penalty = await _context.PenaltyTickets.Include(p => p.Reader).FirstOrDefaultAsync(p => p.IdPenalty == idPenaltyTicket);
            if (penalty == null)
            {
                return ApiResponse<PenaltyTicketResponse>.FailResponse("Không tìm thấy phiếu thu!", 404);
            }

            var reader = penalty.Reader;
            decimal previousCollected = penalty.AmountCollected;
            decimal proposedTotal = reader.TotalDebt + previousCollected - request.AmountCollected;

            // Áp dụng quy định kiểm tra số tiền thu
            int policyValue = await _parameterService.getValueAsync("LateReturnPenaltyPolicy");
            if (policyValue == 1 && request.AmountCollected > (reader.TotalDebt + previousCollected))
            {
                return ApiResponse<PenaltyTicketResponse>.FailResponse("Số tiền thu vượt quá số tiền độc giả đang nợ!", 400);
            }

            penalty.AmountCollected = request.AmountCollected;
            penalty.AmountRemaining = proposedTotal;
            reader.TotalDebt = proposedTotal;

            // Update tổng nợ và tiền đã thu của phiếu thu tiền phạt
            _context.PenaltyTickets.Attach(penalty);
            _context.Entry(penalty).Property(p => p.AmountCollected).IsModified = true;
            _context.Entry(penalty).Property(p => p.AmountRemaining).IsModified = true;

            // Update tổng nợ của reader
            _context.Readers.Attach(reader);
            _context.Entry(reader).Property(r => r.TotalDebt).IsModified = true;
            await _context.SaveChangesAsync();

            return ApiResponse<PenaltyTicketResponse>.SuccessResponse("Cập nhật phiếu thu thành công!", 200, new PenaltyTicketResponse
            {
                IdPenalty = penalty.IdPenalty,
                CreatedDate = penalty.CreatedDate,
                AmountCollected = penalty.AmountCollected,
                AmountRemaining = penalty.AmountRemaining,
                ReaderResponse = new ReaderInPenaltyTicket
                {
                    IdReader = reader.IdReader,
                    NameReader = reader.NameReader
                }
            });
        }
    }
}
