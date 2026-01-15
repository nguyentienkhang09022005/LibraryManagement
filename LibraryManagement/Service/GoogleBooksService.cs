using System.Text.Json;
using System.Text.Json.Serialization;

namespace LibraryManagement.Service
{
    public class GoogleBooksService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://www.googleapis.com/books/v1/volumes";

        public GoogleBooksService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GoogleBooksResponse?> SearchBooksAsync(string query, int maxResults = 40)
        {
            var url = $"{BaseUrl}?q={Uri.EscapeDataString(query)}&maxResults={maxResults}";
            
            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<GoogleBooksResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching from Google Books API: {ex.Message}");
                return null;
            }
        }

        public async Task<List<GoogleBookItem>> SearchBySubjectsAsync(string[] subjects, int booksPerSubject = 10)
        {
            var allBooks = new List<GoogleBookItem>();
            
            foreach (var subject in subjects)
            {
                Console.WriteLine($"Fetching books for subject: {subject}");
                var response = await SearchBooksAsync($"subject:{subject}", booksPerSubject);
                
                if (response?.Items != null)
                {
                    allBooks.AddRange(response.Items);
                }
                
                // Delay to avoid rate limiting
                await Task.Delay(500);
            }
            
            return allBooks;
        }
    }

    // DTOs for Google Books API Response
    public class GoogleBooksResponse
    {
        public int TotalItems { get; set; }
        public List<GoogleBookItem>? Items { get; set; }
    }

    public class GoogleBookItem
    {
        public string? Id { get; set; }
        public VolumeInfo? VolumeInfo { get; set; }
    }

    public class VolumeInfo
    {
        public string? Title { get; set; }
        public List<string>? Authors { get; set; }
        public string? Publisher { get; set; }
        public string? PublishedDate { get; set; }
        public string? Description { get; set; }
        public List<string>? Categories { get; set; }
        public ImageLinks? ImageLinks { get; set; }
        public string? Language { get; set; }
        public int? PageCount { get; set; }
    }

    public class ImageLinks
    {
        public string? SmallThumbnail { get; set; }
        public string? Thumbnail { get; set; }
    }
}
