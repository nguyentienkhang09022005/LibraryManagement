using System.Diagnostics.Contracts;

namespace LibraryManagement.Dto.Request
{
    public class MessageClient
    {

        public string ReceiveUserId {  get; set; }
        public string ReceiveUserName {  get; set; }
        public string AvatarUrl { get; set; }
    }
}
