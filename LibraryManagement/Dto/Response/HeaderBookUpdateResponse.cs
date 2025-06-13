namespace LibraryManagement.Dto.Response
{
    public class HeaderBookUpdateResponse
    {
        public TypeBookResponse TypeBook { get; set; }
        public string NameHeaderBook { get; set; }
        public string DescribeBook { get; set; }
        public List<AuthorOfBookUpdateResponse> Authors { get; set; }
        public BookUpdateResponse bookResponse { get; set; }
    }
    public class BookUpdateResponse
    {
        public string IdBook { get; set; }
        public string NameBook { get; set; }
        public string Publisher { get; set; }
        public int ReprintYear { get; set; }
        public decimal ValueOfBook { get; set; }
        public string? UrlImage { get; set; }
    }
    public class AuthorOfBookUpdateResponse
    {
        public Guid IdAuthor { get; set; }
        public string NameAuthor { get; set; }
    }
}
