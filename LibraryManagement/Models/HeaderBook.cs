using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Models
{
    public class HeaderBook
    {
        [Key]
        [Column(name:"id_headerbook")]
        public Guid IdHeaderBook { get; set; }

        [Column("id_typebook")]
        public Guid IdTypeBook { get; set; }

        [Column("name_headerbook")]
        public string NameHeaderBook { get; set; }

        [Column("describe_book")]
        public string? DescribeBook { get; set; }


        [ForeignKey("IdTypeBook")]
        public TypeBook TypeBook { get; set; }

        public ICollection<BookWriting> bookWritings { get; set; }

        public ICollection<Book> Books { get; set; }
    }

}
