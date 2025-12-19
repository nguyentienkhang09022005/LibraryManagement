using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Service.InterFace;

namespace LibraryManagement.Service
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _geminiApiUrlBase;
        public GeminiService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["GeminiSettings:ApiKey"]
                      ?? throw new ArgumentNullException("Gemini:ApiKey not configured.");
            _geminiApiUrlBase = config["GeminiSettings:BaseUrl"]
                                ?? throw new ArgumentNullException("Gemini:BaseUrl not configured.");
        }

        public async Task<string> GenerateChatResponseAsync(string systemInstruction,
                                                            List<MessageHistoryItem> history,
                                                            string userMessage)
        {
            var geminiRequest = new GeminiRequest
            {
                // Hướng dẫn hệ thống
                SystemInstruction = new SystemInstruction
                {
                    Parts = new List<GeminiPart> { new GeminiPart { Text = systemInstruction } }
                },
                Contents = new List<GeminiContent>()
            };

            // Thêm lịch sử trò chuyện
            foreach (var item in history)
            {
                geminiRequest.Contents.Add(new GeminiContent
                {
                    Role = item.Role.ToLower(), // "user" hoặc "model"
                    Parts = new List<GeminiPart> { new GeminiPart { Text = item.Message } }
                });
            }

            // Khởi tạo tin nhắn mới
            geminiRequest.Contents.Add(new GeminiContent
            {
                Role = "user",
                Parts = new List<GeminiPart> { new GeminiPart { Text = userMessage } }
            });

            string fullUrl = $"{_geminiApiUrlBase}?key={_apiKey}";
            var response = await _httpClient.PostAsJsonAsync(fullUrl, geminiRequest);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                // Log lỗi 
                throw new Exception($"Gemini API call failed: {response.StatusCode} - {errorBody}");
            }

            var apiResponse = await response.Content.ReadFromJsonAsync<GeminiResponse>();

            // Trích xuất văn bản từ phản hồi
            var generatedText = apiResponse?.Candidates?.FirstOrDefault()
                                ?.Content?.Parts?.FirstOrDefault()?.Text;

            return generatedText ?? "Xin lỗi, tôi không thể tạo phản hồi vào lúc này.";
        }
    }
}
