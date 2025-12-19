namespace LibraryManagement.Dto.Request
{
    public class ChatRequest
    {
        public string IdReader { get; set; }

        public string ReaderMessage { get; set; }
    }

    public class MessageHistoryItem
    {
        public string Role { get; set; }

        public string Message { get; set; }
    }
}
