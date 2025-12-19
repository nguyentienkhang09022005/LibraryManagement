using LibraryManagement.Dto.Request;
using LibraryManagement.Models;
using LibraryManagement.Service.InterFace;

namespace LibraryManagement.Service
{
    public class ChatService : IChatService
    {
        private readonly IMessageRepository _messageRepo;
        private readonly IMessageHubService _messageHubService;
        public ChatService(IMessageRepository repo, IMessageHubService messageHubService)
        {
            _messageRepo = repo;
            _messageHubService = messageHubService;
        }

        public Task<List<MessageClient>> getAllMessageClient(string senderId)
        => _messageRepo.getAllMessageClient(senderId);  

        public Task<List<Message>> GetChatHistoryAsync(string readerId1, string readerId2)
            => _messageRepo.GetChatHistoryAsync(readerId1, readerId2);

        public async Task SendMessageAsync(Message message)
        {
            await _messageRepo.SendMessageAsync(message);

            await _messageHubService.PushMessageAsync(message);
        }
    }
}
