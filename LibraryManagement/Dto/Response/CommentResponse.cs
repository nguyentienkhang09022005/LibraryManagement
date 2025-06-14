namespace LibraryManagement.Dto.Response
{
    public class CommentResponse
    {
        public string IdEvaluation { get; set; }
        public string IdBook{ get; set; }
        public string IdReader {  get; set; }
        public string NameReader { get; set; }
        public string AvatarUrl { get; set; }
        public string Comment { get; set; }
        public int Star {  get; set; }
        public DateTime CreateDate {  get; set; }

    }
}
