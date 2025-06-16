namespace LibraryManagement.Dto.Response
{
    public class ReaderResponse
    {
        public string IdReader { get; set; }
        public TypeReaderResponse? IdTypeReader { get; set; }
        public string NameReader { get; set; }
        public string Sex { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public DateTime Dob { get; set; }
        public string Phone { get; set; }
        public DateTime CreateDate { get; set; }
        public string ReaderAccount { get; set; }
        public decimal TotalDebt { get; set; }
        public string? UrlAvatar { get; set; }
        public string Role { get; set; }
    }
}
