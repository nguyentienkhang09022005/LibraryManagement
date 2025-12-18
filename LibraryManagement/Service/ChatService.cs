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

        public Task<List<Message>> GetChatWithReaderAsync(string userId1, string userId2)
            => _messageRepo.GetChatWithReaderAsync(userId1, userId2);

        public Task<List<Message>> GetChatWithManagerAsync(string userId1)
            => _messageRepo.GetChatWithManagerAsync(userId1);

        public Task SendMessageAsync(Message message)
            => _messageRepo.SendMessageAsync(message);
    }
}
