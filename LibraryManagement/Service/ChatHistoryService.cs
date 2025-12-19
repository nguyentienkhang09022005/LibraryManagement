using LibraryManagement.Dto.Request;
using LibraryManagement.Service.InterFace;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace LibraryManagement.Service
{
    public class ChatHistoryService : IChatHistoryService
    {
        private readonly IDistributedCache _cache;
        private readonly JsonSerializerOptions _jsonOptions;

        public ChatHistoryService(IDistributedCache cache)
        {
            _cache = cache;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
        }

        private string GetCacheKey(string idReader) => $"chat_history:{idReader}";

        public async Task DeleteHistoryAsync(string idReader)
        {
            await _cache.RemoveAsync(GetCacheKey(idReader));
        }

        public async Task<List<MessageHistoryItem>> GetHistoryAsync(string idReader)
        {
            var key = GetCacheKey(idReader);
            var jsonData = await _cache.GetStringAsync(key);
            if (string.IsNullOrEmpty(jsonData))
                return new List<MessageHistoryItem>();

            return JsonSerializer.Deserialize<List<MessageHistoryItem>>(jsonData, _jsonOptions)!;
        }

        public async Task SaveMessageAsync(string idReader, MessageHistoryItem message)
        {
            var key = GetCacheKey(idReader);
            var existingData = await _cache.GetStringAsync(key);

            List<MessageHistoryItem> history;
            if (!string.IsNullOrEmpty(existingData))
            {
                history = JsonSerializer.Deserialize<List<MessageHistoryItem>>(existingData, _jsonOptions)!;
            }
            else
            {
                history = new List<MessageHistoryItem>();
            }

            history.Add(message);

            var jsonData = JsonSerializer.Serialize(history, _jsonOptions);

            await _cache.SetStringAsync(
                key,
                jsonData,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                });
        }
    }
}
