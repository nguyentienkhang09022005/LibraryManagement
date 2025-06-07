using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Models
{
    public class Book
    {
        [Key]
        [Column("id_book")]
        public string IdBook { get; set; }

        [Column("id_headerbook")]
        public Guid IdHeaderBook { get; set; }

        [Column("publisher")]
        public string Publisher { get; set; }

        [Column("reprint_year")]
        public int ReprintYear { get; set; }

        [Column("valueofbook")] 
        public decimal ValueOfBook { get; set; }

        [ForeignKey("IdHeaderBook")]
        public HeaderBook HeaderBook { get; set; }

        public ICollection<Evaluate> Evaluates { get; set; }

        public ICollection<FavoriteBook> FavoriteBooks { get; set; }

        public ICollection<Image> images { get; set; }

        
    }

}
