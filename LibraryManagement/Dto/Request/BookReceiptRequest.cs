namespace LibraryManagement.Dto.Request
{
    public class BookReceiptRequest
    {
        public HeaderBookCreationRequest headerBook { get; set; }
        public string IdReader { get; set; }
        public DetailBookReceiptRequest detailsRequest { get; set; }
    }

    public class DetailBookReceiptRequest
    {
        public int Quantity { get; set; }
    }
}
