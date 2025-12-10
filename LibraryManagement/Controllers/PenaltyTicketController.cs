using LibraryManagement.Dto.Request;
using LibraryManagement.Repository.InterFace;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [HttpPost("add-penalty")]
        public async Task<IActionResult> addPenaltyTicket([FromBody] PenaltyTicketRequest request)
        {
            var result = await _penaltyTicketService.addPenaltyTicketAsync(request);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpDelete("delete-penalty")]
        public async Task<IActionResult> deletePenaltyTicket([FromQuery] Guid idPenaltyTicket)
        {
            var result = await _penaltyTicketService.deletePenaltyTicketAsync(idPenaltyTicket);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpPatch("update_penalty")]
        public async Task<IActionResult> updatePenaltyTicket([FromBody] PenaltyTicketRequest request, [FromQuery] Guid idPenaltyTicket)
        {
            var result = await _penaltyTicketService.updatePenaltyTicketAsync(request, idPenaltyTicket);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpGet("list-penalty-ticket-by-user")]
        public async Task<IActionResult> getPenaltiesById([FromQuery] string idUser)
        {
            var result = await _penaltyTicketService.GetTicketResponsesAsync(idUser);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }
    }
}
