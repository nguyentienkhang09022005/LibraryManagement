using LibraryManagement.Dto.Request;
using LibraryManagement.Models;

namespace LibraryManagement.Service.InterFace
{
    public interface IMessageRepository
    {
        public Task SendMessageAsync(Message message);

        Task<List<Message>> GetChatHistoryAsync(string readerId1, string readerId2);

        public Task<List<MessageClient>> getAllMessageClient(string senderId);
    }

}
