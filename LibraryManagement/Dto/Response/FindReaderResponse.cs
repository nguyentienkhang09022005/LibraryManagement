using FluentEmail.Core;

namespace LibraryManagement.Dto.Response
{
    public class FindReaderResponse
    {
        public string phone { get; set; } = null!;

        public string Email { get; set; } = null!;

        public DateTime birthday { get; set; }

        public string password { get; set; } = null!; 

        public DateTime DateCreate { get; set; }

        public DateTime DateExpired { get; set; }
    }
}
