namespace Biobanks.Submissions.Models
{
    public class CommitQueueItem
    {
        public int BiobankId { get; set; }
        public bool Replace { get; set; }
    }
}
