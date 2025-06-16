using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Models
{
    public class Reader
    {
        [Key]
        [Column("id_reader")]
        public string IdReader { get; set; }

        [Column("id_typereader")]
        public Guid IdTypeReader { get; set; }

        [Column("name_reader")]
        public string? NameReader { get; set; }

        [Column("sex")]
        public string? Sex { get; set; }

        [Column("address")]
        public string? Address { get; set; }

        [Column("email")]
        public string? Email { get; set; }

        [Column("phone")]
        public string? Phone { get; set; }

        [Column("dob")]
        public DateTime Dob { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; } 

        [Column("reader_username")]
        public string? ReaderUsername { get; set; }

        [Column("reader_password")]
        public string? ReaderPassword { get; set; }

        [Column("total_debt")]
        public decimal TotalDebt { get; set; }

        [Column("check_login")]
        public int CheckLogin { get; set; }

        [ForeignKey("IdTypeReader")]
        public TypeReader TypeReader { get; set; }

        [Column("role_name")]
        public string RoleName { get; set; }

        [ForeignKey("RoleName")]
        public Role Role { get; set; }

        public ICollection<FavoriteBook> likedHeaderBooks { get; set; }
        public ICollection<Image> Images { get; set; }
    }

}
