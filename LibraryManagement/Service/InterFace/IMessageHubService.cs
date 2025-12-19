using LibraryManagement.Models;

namespace LibraryManagement.Service.InterFace
{
    public interface IMessageHubService
    {
        Task PushMessageAsync(Message message);
    }
}
