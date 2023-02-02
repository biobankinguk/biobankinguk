namespace Biobanks.Entities.Api
{
    public class Error
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public string RecordIdentifiers { get; set; }

        public int SubmissionId { get; set; }
        public Submission Submission { get; set; }
    }
}
