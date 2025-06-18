namespace LibraryManagement.Dto.Request
{
    public class TicketResponse
    {
        public DateTime CreatedDate { get; set; }
        public decimal TotalDebit {  get; set; }
        public decimal AmountCollected { get; set; }
        public decimal AmountRemaining { get; set; }
    }
}
