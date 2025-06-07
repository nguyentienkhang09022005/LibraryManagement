namespace LibraryManagement.Dto.Response
{
    public class FindBookResponse
    {
        public string idBook { get; set; }
        public string nameBook { get; set; }
        public string describe { get; set; }

        public string image { get; set; }

        public bool isLiked { get; set; } = false;
    }
}
