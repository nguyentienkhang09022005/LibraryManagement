using LibraryManagement.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace LibraryManagement.Dto.Request
{
    public class MessageRequest
    {
        public string? ReceiverId { get; set; }

        public MessageContent Content { get; set; } = null!;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
