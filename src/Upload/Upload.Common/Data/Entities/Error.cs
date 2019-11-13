namespace Upload.Common.Data.Entities
{
    public class Error
    {
        public int Id { get; set; }

        public string Message { get; set; } = string.Empty;

        public string RecordIdentifiers { get; set; } = string.Empty;

        public int SubmissionId { get; set; }
        public virtual Submission Submission { get; set; } = null!;
    }
}