
namespace LibraryManagement.Dto.Response
{
    public class BookReceiptInformationOutput
    {
        public string? IdReader { get; set; }

        public string? ReaderName { get; set; } = null!;

        public DateTime? receivedDate { get; set; }

        public int? Quantity { get; set; }

        public decimal? unitprice { get; set; }

        public string? IdBook { get; set; } = null!;
    }
}
