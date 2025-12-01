using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Dto.Request
{
    public class AuthenticationRequest
    {
        [Required]
        public string email { get; set; }

        [Required]
        public string password { get; set; }
    }
}
