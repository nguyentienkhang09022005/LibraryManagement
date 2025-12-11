using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Models
{
    public class LoanSlipBook
    {
        [Key]
        [Column("id_loanslipbook")]
        public Guid IdLoanSlipBook { get; set; }

        [Column("id_thebook")]
        public string IdTheBook { get; set; }

        [Column("id_reader")]
        public string IdReader { get; set; }

        [Column("borrow_date")]
        public DateTime BorrowDate { get; set; }

        [Column("return_date")]
        public DateTime ReturnDate { get; set; }

        [Column("loan_period")]
        public int LoanPeriod { get; set; }

        [Column("fine_amount")]
        public decimal FineAmount { get; set; }

        [Column("is_returned")]
        public bool IsReturned { get; set; } = false;

        [ForeignKey("IdReader")]
        public Reader Reader { get; set; }

        [ForeignKey("IdTheBook")]
        public TheBook TheBook { get; set; } 
    }
}
