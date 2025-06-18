using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Models
{
    public class InvalidateToken
    {
        [Key]
        [Column("id_token")]
        public string IdToken { get; set; }

        [Column("expiry_time")]
        public DateTime ExpiryTime { get; set; }
    }
}
