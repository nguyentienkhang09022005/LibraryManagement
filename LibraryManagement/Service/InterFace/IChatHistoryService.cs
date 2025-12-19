using LibraryManagement.Dto.Request;

namespace LibraryManagement.Service.InterFace
{
    public interface IChatHistoryService
    {
        Task SaveMessageAsync(string idReader, MessageHistoryItem message);

        Task<List<MessageHistoryItem>> GetHistoryAsync(string idReader);

        Task DeleteHistoryAsync(string idReader);
    }
}
