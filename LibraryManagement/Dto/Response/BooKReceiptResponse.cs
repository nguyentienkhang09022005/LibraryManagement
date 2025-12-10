namespace LibraryManagement.Dto.Response
{
    public class BooKReceiptResponse
    {
        public Guid IdBookReceipt { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public List<DetailBookReceiptResponse>? listDetailsResponse { get; set; }
    }
    public class DetailBookReceiptResponse
    {
        public int? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
    }
}
