namespace LibraryManagement.Dto.Request
{
    public class ReaderCreationRequest
    {
        public Guid IdTypeReader { get; set; }
        public string NameReader { get; set; }
        public string Sex { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public DateTime Dob { get; set; }
        public string Phone { get; set; }
        public string ReaderPassword { get; set; }
        public IFormFile AvatarImage { get; set; }
    }
}
