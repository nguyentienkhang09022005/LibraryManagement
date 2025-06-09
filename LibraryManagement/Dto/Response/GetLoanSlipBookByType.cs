using LibraryManagement.Models;

namespace LibraryManagement.Dto.Response
{
    public class GetLoanSlipBookByType
    {
        public Guid IdReportGenre {  get; set; }  
        public int ReportMonth { get; set; }

        public string Genre { get; set; }
        public int TotalBorrow { get; set; }    

        public double Rate { get; set; }
    }
}
