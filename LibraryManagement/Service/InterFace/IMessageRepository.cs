using LibraryManagement.Dto.Request;
using LibraryManagement.Models;

namespace LibraryManagement.Service.InterFace
{
    public interface IMessageRepository
    {
        public Task SendMessageAsync(Message message);

        Task<List<Message>> GetChatWithReaderAsync(string readerId, string managerId);

        Task<List<Message>> GetChatWithManagerAsync(string readerId);

        public Task<List<MessageClient>> getAllMessageClient(string senderId);
    }

}
