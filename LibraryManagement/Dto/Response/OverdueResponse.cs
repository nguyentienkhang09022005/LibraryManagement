namespace LibraryManagement.Dto.Response
{
    public class OverdueResponse
    {
        public Guid IdOverDue{ get; set; }
        public DateTime ReportDate { get; set; }
        public string IdBook {  get; set; }
        public string NameBook {  get; set; }
        public DateTime DateBorrow { get; set; }
        public int DateLate { get; set; }

        public string IdUser {  get; set; }
        public string UserName{ get; set; }
        public string TotalFine { get; set; }
    }
}
