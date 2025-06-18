namespace LibraryManagement.Dto.Response
{
    public class GetHeaderbookResponse
    {
        public Guid IdHeaderbook { get; set; } 
        public Guid IdTypeBook { get; set; }
        public string NameBook { get; set; } = null!;
        public string Describe { get; set; } = null!;
    }
}
