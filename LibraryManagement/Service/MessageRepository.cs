using LibraryManagement.Models;
using LibraryManagement.Service.InterFace;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;

namespace LibraryManagement.Service
{
    public class MessageRepository : IMessageRepository
    {
       
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IMongoCollection<Message> _messages;

        public MessageRepository(IConfiguration configuration, IHubContext<ChatHub> hubContext )
        {
            var connectionString = configuration["MongoDB:ConnectionString"];
            var databaseName = configuration["MongoDB:DatabaseName"];
            var collectionName = configuration["MongoDB:MessagesCollection"];

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _messages = database.GetCollection<Message>(collectionName);
            _hubContext = hubContext;
        }

        public async Task<List<Message>> GetAllMessagesAsync(string userId1, string userId2)
        {
            var filter = Builders<Message>.Filter.Or(
                Builders<Message>.Filter.And(
                    Builders<Message>.Filter.Eq(m => m.SenderId, userId1),
                    Builders<Message>.Filter.Eq(m => m.ReceiverId, userId2)
                ),
                Builders<Message>.Filter.And(
                    Builders<Message>.Filter.Eq(m => m.SenderId, userId2),
                    Builders<Message>.Filter.Eq(m => m.ReceiverId, userId1)
                )
            );

            return await _messages.Find(filter)
                .SortBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task SendMessageAsync(Message message)
        {
            message.SentAt = DateTime.UtcNow;
            await _messages.InsertOneAsync(message);

            // Push tin nhắn realtime bằng SignalR
            await _hubContext.Clients.User(message.ReceiverId)
                .SendAsync("ReceiveMessage", message.SenderId, message.Content, message.SentAt);
        }
    }
}
