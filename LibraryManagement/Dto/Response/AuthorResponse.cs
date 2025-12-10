namespace LibraryManagement.Dto.Response
{
    public class AuthorResponse
    {
        public Guid IdAuthor { get; set; }

        public TypeBookResponse? IdTypeBook { get; set; }

        public string? NameAuthor { get; set; }

        public string? Nationality { get; set; }

        public string? Biography { get; set; }

        public string? UrlAvatar { get; set; }
    }
}
