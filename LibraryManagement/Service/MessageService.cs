using LibraryManagement.Models;
using MongoDB.Driver;

namespace LibraryManagement.Service
{
    public class MessageService
    {
        private readonly IMongoCollection<Message> _messages;

        public MessageService(IConfiguration configuration)
        {
            var connectionString = configuration["MongoDB:ConnectionString"];
            var databaseName = configuration["MongoDB:DatabaseName"];
            var collectionName = configuration["MongoDB:MessagesCollection"];

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _messages = database.GetCollection<Message>(collectionName);
        }

        public async Task<List<Message>> GetMessagesBetween(string senderId, string receiverId)
        {
            var filter = Builders<Message>.Filter.Or(
                Builders<Message>.Filter.And(
                    Builders<Message>.Filter.Eq(m => m.SenderId, senderId),
                    Builders<Message>.Filter.Eq(m => m.ReceiverId, receiverId)
                ),
                Builders<Message>.Filter.And(
                    Builders<Message>.Filter.Eq(m => m.SenderId, receiverId),
                    Builders<Message>.Filter.Eq(m => m.ReceiverId, senderId)
                )
            );
            return await _messages.Find(filter).SortBy(m => m.SentAt).ToListAsync();
        }

        public async Task SendMessageAsync(Message msg)
        {
            await _messages.InsertOneAsync(msg);
        }
    }
}
