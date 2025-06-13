using LibraryManagement.Dto.Request;
using LibraryManagement.Repository.InterFace;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PenaltyTicketController : ControllerBase
    {
        private readonly IPenaltyTicketService _penaltyTicketService;

        public PenaltyTicketController(IPenaltyTicketService penaltyTicketService)
        {
            _penaltyTicketService = penaltyTicketService;
        }

        // Endpoint tạo phiếu thu tiền phạt
        [HttpPost("add_penalty")]
        public async Task<IActionResult> addPenaltyTicket([FromBody] PenaltyTicketRequest request)
        {
            var result = await _penaltyTicketService.addPenaltyTicketAsync(request);
            if (result.Success)
                return Created("", result);
            return BadRequest(result);
        }

        // Endpoint xóa phiếu thu tiền phạt
        [HttpDelete("delete_penalty/{idPenaltyTicket}")]
        public async Task<IActionResult> deletePenaltyTicket(Guid idPenaltyTicket)
        {
            var result = await _penaltyTicketService.deletePenaltyTicketAsync(idPenaltyTicket);
            if (result.Success)
                return Ok(result);
            return NotFound(result);
        }

        // Endpoint sửa thông tin phiếu thu tiền phạt
        [HttpPatch("update_penalty/{idPenaltyTicket}")]
        public async Task<IActionResult> updatePenaltyTicket([FromBody] PenaltyTicketRequest request, Guid idPenaltyTicket)
        {
            var result = await _penaltyTicketService.updatePenaltyTicketAsync(request, idPenaltyTicket);
            if (result.Success)
                return Ok(result);
            return NotFound(result);
        }
    }
}
