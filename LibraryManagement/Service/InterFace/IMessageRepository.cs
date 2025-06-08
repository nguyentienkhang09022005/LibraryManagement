using LibraryManagement.Models;

namespace LibraryManagement.Service.InterFace
{
    public interface IMessageRepository
    {
        public Task SendMessageAsync(Message message);
        public Task<List<Message>> GetAllMessagesAsync(string userId1, string userId2);

    }

}
