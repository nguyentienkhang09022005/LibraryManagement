namespace LibraryManagement.Dto.Response
{
    public class BooksAndCommentsResponse
    {
        public string idBook { get; set; }

        public string nameBook { get; set; }

        public string describe { get; set; }

        public decimal valueOfbook { get; set; }

        public string Publisher { get; set; }

        public string image { get; set; }

        public int reprintYear { get; set;  }

        public List<EvaluationDetails> Evaluations { get; set; }

        public List<AuthorResponse> Authors { get; set; }
    }
}
