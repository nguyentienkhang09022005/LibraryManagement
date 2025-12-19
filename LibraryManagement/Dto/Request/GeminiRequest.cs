using System.Text.Json.Serialization;

namespace LibraryManagement.Dto.Request
{
    public class GeminiRequest
    {
        [JsonPropertyName("contents")]
        public List<GeminiContent> Contents { get; set; }

        [JsonPropertyName("systemInstruction")]
        public SystemInstruction SystemInstruction { get; set; }
    }

    public class SystemInstruction
    {
        [JsonPropertyName("parts")]
        public List<GeminiPart> Parts { get; set; }
    }

    public class GeminiContent
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("parts")]
        public List<GeminiPart> Parts { get; set; }
    }

    public class GeminiPart
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}
