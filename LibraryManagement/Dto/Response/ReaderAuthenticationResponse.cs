namespace LibraryManagement.Dto.Response
{
    public class ReaderAuthenticationResponse
    {
        public string IdReader { get; set; }           // id_reader
        public Guid IdTypeReader { get; set; }         // id_typereader
        public string? NameReader { get; set; }        // name_reader
        public string? Sex { get; set; }               // sex
        public string? Address { get; set; }           // address
        public string? Email { get; set; }             // email
        public DateTime Dob { get; set; }              // dob
        public DateTime CreateDate { get; set; }       // create_date
        public DateTime ExpiryDate { get; set; }       // expiry_date
        public string ReaderUsername { get; set; }     // reader_username
        public string RoleName { get; set; }           // role_name
        public string? TypeReader { get; set; }
        public string AvatarUrl { get; set; }
    }
}
