using LibraryManagement.Models;
using LibraryManagement.Service.InterFace;
using Microsoft.AspNetCore.SignalR;

namespace LibraryManagement.Service
{
    public class MessageHubService : IMessageHubService
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public MessageHubService(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task PushMessageAsync(Message message)
        {
            await _hubContext.Clients.User(message.ReceiverId)
                .SendAsync("ReceiveMessage", message.SenderId, message.Content, message.SentAt);
        }
    }
}
