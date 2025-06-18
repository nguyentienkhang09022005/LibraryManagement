using LibraryManagement.Dto.Request;
using LibraryManagement.Models;

namespace LibraryManagement.Service.InterFace
{
    public interface IMessageRepository
    {
        public Task SendMessageAsync(Message message);
        public Task<List<Message>> GetAllMessagesAsync(string userId1, string userId2);
        public Task<List<MessageClient>> getAllMessageClient(string senderId);
    }

}
