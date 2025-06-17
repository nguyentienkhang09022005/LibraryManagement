namespace LibraryManagement.Dto.Response
{
    public class CategoryOverdueResponse
    {
        public Guid IdOverDueReport { get; set; }
        public DateTime reportDate { get; set; }
        public string IDbook { get; set; }
        public string BookName { get; set; }
        public DateTime DateBorrow { get; set; }
        public int DateLate { get; set; }
    }
}
