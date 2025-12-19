using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers;

namespace LibraryManagement.Service.InterFace
{
    public interface IChatWithAIService
    {
        Task<ApiResponse<ChatResponse>> SendMessageForAI(ChatRequest request);

        Task<ApiResponse<List<MessageHistoryItem>>> GetChatHistoryAsync(string idReader);

        Task<ApiResponse<string>> DeleteChatHistoryAsync(string idReader);
    }
}
