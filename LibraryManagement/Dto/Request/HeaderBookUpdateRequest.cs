namespace LibraryManagement.Dto.Request
{
    public class HeaderBookUpdateRequest
    {
        public Guid? IdTypeBook { get; set; }
        public string? NameHeaderBook { get; set; }
        public string? DescribeBook { get; set; }
        public List<Guid>? IdAuthors { get; set; }
        public IFormFile? BookImage { get; set; }
        public BookUpdateRequest? bookUpdateRequest { get; set; }
        public TheBookUpdateRequest? theBookUpdateRequest { get; set; }
    }

    public class BookUpdateRequest
    {
        public string? Publisher { get; set; }
        public int? ReprintYear { get; set; }
        public decimal? ValueOfBook { get; set; }
    }

    public class TheBookUpdateRequest
    {
        public string? Status { get; set; }
    }
}
