namespace LibraryManagement.Dto.Response
{
    public class SlipBookResponse
    {
        public Guid IdLoanSlipBook { get; set; }
        public string? IdTheBook { get; set; }
        public ReaderInSlipBookResponse? readerResponse { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public int LoanPeriod { get; set; }
        public decimal FineAmount { get; set; }
    }
    public class ReaderInSlipBookResponse
    {
        public string IdReader { get; set; }
        public string NameReader { get; set; }
        public decimal TotalDebt { get; set; }
    }
}
