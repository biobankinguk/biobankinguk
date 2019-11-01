namespace Common.Data.Upload
{
    public class Error
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public string RecordIdentifiers { get; set; }

        public int SubmissionId { get; set; }
        public virtual Submission Submission { get; set; }
    }
}