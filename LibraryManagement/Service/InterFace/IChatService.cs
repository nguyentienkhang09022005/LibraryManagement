using LibraryManagement.Dto.Request;
using LibraryManagement.Models;

namespace LibraryManagement.Service.InterFace
{
    public interface IChatService
    {
        Task<List<Message>> GetChatHistoryAsync(string readerId1, string readerId2);

        Task SendMessageAsync(Message message);

        Task<List<MessageClient>> getAllMessageClient(string senderId);
    }
}
