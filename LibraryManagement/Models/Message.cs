using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Models
{
    public class Message
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }

        [BsonElement("senderId")]
        public string SenderId { get; set; } = null!;

        [BsonElement("receiverId")]
        public string ReceiverId { get; set; } = null!;

        [BsonElement("content")]
        public MessageContent Content { get; set; } = null!;

        [BsonElement("sentAt")]
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }

    public class MessageContent
    {
        [BsonElement("type")]
        public string Type { get; set; } = "text"; // "text", "image", "file"

        [BsonElement("data")]
        public string Data { get; set; } = null!;
    }
}
