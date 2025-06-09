using Microsoft.AspNetCore.SignalR;

namespace LibraryManagement.Service.InterFace
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string toUser, string message)
        {
            await Clients.User(toUser).SendAsync("ReceiveMessage", Context.UserIdentifier, message);
        }
    }
}
