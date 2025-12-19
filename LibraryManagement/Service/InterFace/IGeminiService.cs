using LibraryManagement.Dto.Request;

namespace LibraryManagement.Service.InterFace
{
    public interface IGeminiService
    {
        Task<string> GenerateChatResponseAsync(string systemInstruction,
                                               List<MessageHistoryItem> history,
                                               string userMessage);
    }
}
