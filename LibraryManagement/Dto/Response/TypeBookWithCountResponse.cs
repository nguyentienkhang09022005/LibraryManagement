namespace LibraryManagement.Dto.Response
{
    public class TypeBookWithCountResponse
    {
        public Guid IdTypeBook { get; set; }
        public string NameTypeBook { get; set; } = string.Empty;
        public int BookCount { get; set; }
    }
}
