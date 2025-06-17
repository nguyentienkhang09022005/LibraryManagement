namespace LibraryManagement.Dto.Response
{
    public class OverdueReportResponse
    {
        public Guid IdOverdueReport { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<OverdueReportDetailResponse> Detail { get; set; }
    }
    public class OverdueReportDetailResponse
    {
        public Guid IdOverdueReport { get; set; }
        public string IdTheBook { get; set; }
        public string NameHeaderBook { get; set; }

        public DateTime BorrowDate { get; set; }

        public int LateDays { get; set; }
    }
}
