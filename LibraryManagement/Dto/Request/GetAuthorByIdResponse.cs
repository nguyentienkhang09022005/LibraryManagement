using LibraryManagement.Dto.Response;

namespace LibraryManagement.Dto.Request
{
    public class GetAuthorByIdResponse
    {
        public Guid IdAuthor { get; set; }
        public TypeBookResponse? IdTypeBook { get; set; }
        public string NameAuthor { get; set; }
        public string Nationality { get; set; }
        public string Biography { get; set; }
        public string? UrlAvatar { get; set; }
        public List<BookResponse> Books { get; set; }

    }
}
