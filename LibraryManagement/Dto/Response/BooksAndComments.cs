namespace LibraryManagement.Dto.Response
{
    public class BooksAndComments
    {
        public string idBook { get; set; }

        public string nameBook { get; set; }

        public string describe { get; set; }

        public decimal valueOfbook {  get; set; }
        public string publisher { get; set; }

        public string image {  get; set; }

        public int reprintYear { get; set; }

        public bool isLiked { get; set; } = false;

        public List<EvaluationDetails> Evaluations { get;set; }

        public List<AuthorResponse> Authors{ get; set; }
 
    }
}
