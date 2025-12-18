using LibraryManagement.Data;
using LibraryManagement.Dto.Request;
using LibraryManagement.Models;
using LibraryManagement.Service.InterFace;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using ZstdSharp.Unsafe;

namespace LibraryManagement.Service
{
    public class MessageRepository : IMessageRepository
    {
       
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IMongoCollection<Message> _messages;
        private readonly LibraryManagermentContext _context; 

        public MessageRepository(IConfiguration configuration, IHubContext<ChatHub> hubContext, LibraryManagermentContext context )
        {
            var connectionString = configuration["MongoDB:ConnectionString"];
            var databaseName = configuration["MongoDB:DatabaseName"];
            var collectionName = configuration["MongoDB:MessagesCollection"];

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _messages = database.GetCollection<Message>(collectionName);
            _hubContext = hubContext;
            _context = context;
            EnsureIndexes();
        }
        private void EnsureIndexes()
        {
            var indexModels = new List<CreateIndexModel<Message>>();
            indexModels.Add(new CreateIndexModel<Message>(
                Builders<Message>.IndexKeys
                    .Ascending(m => m.SenderId)
                    .Ascending(m => m.ReceiverId)
                    .Ascending(m => m.SentAt)
            ));
            indexModels.Add(new CreateIndexModel<Message>(
                Builders<Message>.IndexKeys
                    .Ascending(m => m.ReceiverId)
                    .Descending(m => m.SentAt)
            ));
            indexModels.Add(new CreateIndexModel<Message>(
                Builders<Message>.IndexKeys
                    .Ascending("GroupId") 
                    .Descending(m => m.SentAt)
            ));

           
            indexModels.Add(new CreateIndexModel<Message>(
                Builders<Message>.IndexKeys.Text(m => m.Content)
            ));

            _messages.Indexes.CreateMany(indexModels);
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
            await _hubContext.Clients.User(message.ReceiverId)
                .SendAsync("ReceiveMessage", message.SenderId, message.Content, message.SentAt);
        }

        public async Task<List<MessageClient>> getAllMessageClient(string senderId)
        {
            // Lấy toàn bộ PartnerId unique chỉ với 1 truy vấn
            var userIds = await _messages.Aggregate()
                .Match(m => m.SenderId == senderId || m.ReceiverId == senderId)
                .Project(m => new
                {
                    PartnerId = m.SenderId == senderId ? m.ReceiverId : m.SenderId
                })
                .Group(x => x.PartnerId, g => new { UserId = g.Key })
                .ToListAsync();

            var allUserIds = userIds
                .Select(x => x.UserId)
                .Where(id => !string.IsNullOrEmpty(id) && id != senderId)
                .ToList();

            if (allUserIds.Count == 0)
                return new List<MessageClient>();

            // Truy vấn nhanh SQL chỉ lấy trường cần
            var userInfos = await _context.Readers
                .AsNoTracking()
                .Where(x => allUserIds.Contains(x.IdReader))
                .Select(x => new { x.IdReader, x.NameReader })
                .ToListAsync();

            var userInfoDict = userInfos.ToDictionary(x => x.IdReader, x => x.NameReader);

            var result = allUserIds
                .Select(id => new MessageClient
                {
                    ReceiveUserId = id,
                    ReceiveUserName = userInfoDict.TryGetValue(id, out var name) ? name : "(Không rõ tên)",
                    AvatarUrl = _context.Images.AsNoTracking().Where(x=>x.IdReader == id).Select(x=>x.Url).FirstOrDefault() ?? string.Empty,
                })
                .ToList();

            return result;
        }


    }
}
