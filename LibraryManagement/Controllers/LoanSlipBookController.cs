using LibraryManagement.Data;
using LibraryManagement.Dto.Request;
using LibraryManagement.Repository.InterFace;
using LibraryManagement.Service.InterFace;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using System.Diagnostics.Contracts;

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanSlipBookController : ControllerBase
    {
        private readonly ILoanBookService _loanBookService;
        private readonly ISlipBookService _slipBookService;
        private readonly LibraryManagermentContext _context;

        public LoanSlipBookController(ILoanBookService loanSlipBookService, LibraryManagermentContext context,
                                                                            ISlipBookService slipBookService)
        {
            _loanBookService = loanSlipBookService;
            _context = context;
            _slipBookService = slipBookService;
        }

        // Enpoint danh sách phiếu mượn trả sách
        [HttpGet("getAllBookLoanSlip")]
        public async Task<IActionResult> getAllBookLoanSlip()
        {
            var result = await _loanBookService.getListLoanSlipBook();
            if (result == null) return Unauthorized("Vui lòng đăng nhập");
            return Ok(result);

        }

        // Enpoint tạo phiếu mượn sách
        [HttpPost("add_loanbook")]
        public async Task<IActionResult> addLoanBook([FromBody] LoanBookRequest request)
        {
            var result = await _loanBookService.addLoanBookAsync(request);
            if (result.Success)
                return Created("", result);
            return BadRequest(result);
        }

        // Endpoint xóa phiếu mượn sách
        [HttpDelete("delete_loanbook/{idLoanSlipBook}")]
        public async Task<IActionResult> deleteLoanBook(Guid idLoanSlipBook)
        {
            var result = await _loanBookService.deleteLoanBookAsync(idLoanSlipBook);
            if (result.Success)
                return Ok(result);
            return NotFound(result);
        }

        // Enpoint tạo phiếu trả sách
        [HttpPost("add_slipbook")]
        public async Task<IActionResult> addSlipBook([FromBody] SlipBookRequest request)
        {
            var result = await _slipBookService.addSlipBookAsync(request);
            if (result.Success)
                return Created("", result);
            return BadRequest(result);
        }

        // Endpoint xóa phiếu trả sách
        [HttpDelete("delete_slipbook/{idLoanSlipBook}")]
        public async Task<IActionResult> deleteSlipBook(Guid idLoanSlipBook)
        {
            var result = await _slipBookService.deleteSlipBookAsync(idLoanSlipBook);
            if (result.Success)
                return Ok(result);
            return NotFound(result);
        }
        [HttpGet("getloansliphistory")]
        public async Task<IActionResult> getLoanSlipByUser(string idUser)
        {
            try
            {
                var result = await _loanBookService.getLoanSlipBookByUser(idUser);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("getloanslipbygenre")]
        public async Task<IActionResult> getLoanslipByType(string? genre)
        {
            try
            {
                var result = await _loanBookService.getLoanSlipBookByType(genre ?? string.Empty);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("getAmountByTypeBook")]
        public async Task<IActionResult> getAmountByTypebook()
        {
            try
            {
                var result = await _loanBookService.getAmountByTypeBook();
                return Ok(result);  
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }

        }
    }
}
