namespace LibraryManagement.Dto.Response
{
    public class TypeBookResponseAndBook
    {
        public string IDTypeBook { get; set; }
        public string TypeBook { get; set; } = string.Empty;
        public string IdHeaderBook { get; set; } = null!;
        public string NameHeaderBook { get; set; } = null!;
    }
}
