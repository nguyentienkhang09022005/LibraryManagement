namespace LibraryManagement.Dto.Response
{
    public class HeaderBookResponse
    {
        public TypeBookResponse TypeBook { get; set; }
        public string NameHeaderBook { get; set; }
        public string DescribeBook { get; set; }
        public List<AuthorOfBookResponse> Authors { get; set; }
        public BookResponse bookResponse { get; set; }
        public TheBookResponse thebookReponse { get; set; }

    }
    public class BookResponse
    {
        public string IdBook { get; set; }
        public string NameBook { get; set; }
        public string Publisher { get; set; }
        public int ReprintYear { get; set; }
        public decimal ValueOfBook { get; set; }
        public string? UrlImage { get; set; }
    }
    public class TheBookResponse
    {
        public string IdTheBook { get; set; }
        public string Status { get; set; }
    }
    public class AuthorOfBookResponse
    {
        public Guid IdAuthor { get; set; }
        public string NameAuthor { get; set; }
    }
}
