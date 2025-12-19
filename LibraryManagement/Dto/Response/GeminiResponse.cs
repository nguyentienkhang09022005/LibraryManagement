using LibraryManagement.Dto.Request;
using System.Text.Json.Serialization;

namespace LibraryManagement.Dto.Response
{
    public class GeminiResponse
    {
        [JsonPropertyName("candidates")]

        public List<GeminiCandidate> Candidates { get; set; }
    }

    public class GeminiCandidate
    {
        [JsonPropertyName("content")]

        public GeminiContent Content { get; set; }
    }
}
