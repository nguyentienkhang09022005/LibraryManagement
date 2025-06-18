using LibraryManagement.Dto.Request;
using LibraryManagement.Models;
using LibraryManagement.Service.InterFace;

namespace LibraryManagement.Service
{
    public class ChatService : IChatService
    {
        private readonly IMessageRepository _messageRepo;
        public ChatService(IMessageRepository repo)
        {
            _messageRepo = repo;
        }

        public Task<List<MessageClient>> getAllMessageClient(string senderId)
        => _messageRepo.getAllMessageClient(senderId);  

        public Task<List<Message>> GetAllMessagesAsync(string userId1, string userId2)
            => _messageRepo.GetAllMessagesAsync(userId1, userId2);

        public Task SendMessageAsync(Message message)
            => _messageRepo.SendMessageAsync(message);
    }
}
