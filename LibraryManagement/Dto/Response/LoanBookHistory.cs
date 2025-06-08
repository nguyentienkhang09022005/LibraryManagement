namespace LibraryManagement.Dto.Response
{
    public class LoanBookHistory
    {
        public string IdBook { get; set; }
        public string NameBook { get; set; }

        public string Genre {  get; set; }
        public DateTime DateBorrow { get; set; }
        public DateTime DateReturn { get; set; }
        public string AvatarUrl{ get; set; }
    }
}
