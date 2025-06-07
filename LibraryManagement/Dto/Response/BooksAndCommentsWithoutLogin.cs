namespace LibraryManagement.Dto.Response
{
    public class BooksAndCommentsWithoutLogin
    {
        public string idBook { get; set; }
        public string nameBook { get; set; }
        public string describe { get; set; }

        public string image { get; set; }



        public List<EvaluationDetails> Evaluations { get; set; }

        public List<AuthorResponse> Authors { get; set; }
    }
}
