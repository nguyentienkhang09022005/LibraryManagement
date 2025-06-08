using LibraryManagement.Models;

namespace LibraryManagement.Service.InterFace
{
    public interface IChatService
    {
        Task<List<Message>> GetAllMessagesAsync(string userId1, string userId2);
        Task SendMessageAsync(Message message);
    }
}
