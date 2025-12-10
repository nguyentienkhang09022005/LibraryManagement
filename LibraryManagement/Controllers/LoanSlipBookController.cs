using LibraryManagement.Dto.Request;
using LibraryManagement.Repository.InterFace;
using LibraryManagement.Service.InterFace;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanSlipBookController : ControllerBase
    {
        private readonly ILoanBookService _loanBookService;
        private readonly ISlipBookService _slipBookService;

        public LoanSlipBookController(ILoanBookService loanSlipBookService, 
                                      ISlipBookService slipBookService)
        {
            _loanBookService = loanSlipBookService;
            _slipBookService = slipBookService;
        }

        [Authorize]
        [HttpGet("get-all-loan-slip-book")]
        public async Task<IActionResult> getAllBookLoanSlip()
        {
            var result = await _loanBookService.getListLoanSlipBook();
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpPost("add-loanbook")]
        public async Task<IActionResult> addLoanBook([FromBody] LoanBookRequest request)
        {
            var result = await _loanBookService.addLoanBookAsync(request);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpDelete("delete-loanbook")]
        public async Task<IActionResult> deleteLoanBook([FromQuery] Guid idLoanSlipBook)
        {
            var result = await _loanBookService.deleteLoanBookAsync(idLoanSlipBook);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpPost("add-slipbook")]
        public async Task<IActionResult> addSlipBook([FromBody] SlipBookRequest request)
        {
            var result = await _slipBookService.addSlipBookAsync(request);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpDelete("delete-slipbook")]
        public async Task<IActionResult> deleteSlipBook([FromQuery] Guid idLoanSlipBook)
        {
            var result = await _slipBookService.deleteSlipBookAsync(idLoanSlipBook);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpGet("get-loan-slip-book-history")]
        public async Task<IActionResult> getLoanSlipByUser([FromQuery] string idUser)
        {
            var result = await _loanBookService.getLoanSlipBookByUser(idUser);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpGet("get-loans-slip-book-by-genre")]
        public async Task<IActionResult> getLoanslipByType([FromQuery] string? genre)
        {
            var result = await _loanBookService.getLoanSlipBookByType(genre);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpGet("get-amount-by-typebook")]
        public async Task<IActionResult> getAmountByTypebook([FromQuery] int month)
        {
            var result = await _loanBookService.getAmountByTypeBook(month);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpGet("get-receipt-history")]
        public async Task<IActionResult> getReceiptHistory([FromQuery] string idReader)
        {
            var result = await _loanBookService.getLoanSlipBookByReader(idReader);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }
    }
}
