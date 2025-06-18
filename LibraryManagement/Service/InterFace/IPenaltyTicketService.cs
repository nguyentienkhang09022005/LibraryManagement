using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers;

namespace LibraryManagement.Repository.InterFace
{
    public interface IPenaltyTicketService
    {
        Task<ApiResponse<PenaltyTicketResponse>> addPenaltyTicketAsync(PenaltyTicketRequest request);
        Task<ApiResponse<PenaltyTicketResponse>> updatePenaltyTicketAsync(PenaltyTicketRequest request, Guid idPenaltyTicket);
        Task<ApiResponse<string>> deletePenaltyTicketAsync(Guid idPenaltyTicket);

        Task<List<TicketResponse>> GetTicketResponsesAsync(string idUser);
    }
}
