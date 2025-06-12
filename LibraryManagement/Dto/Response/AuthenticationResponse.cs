namespace LibraryManagement.Dto.Response
{
    public class AuthenticationResponse
    {
        public string Token { get; set; }
        public string refreshToken { get; set; }

        public string iduser { get; set; }
    }
}
