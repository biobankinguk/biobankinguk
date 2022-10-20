namespace Biobanks.Submissions.Api.Utilities
{
    public class FeedbackMessage
    {
        public string Message { get; set; }
        public FeedbackMessageType Type { get; set; }
        public bool ContainsHtml { get; set; }
    }

    public enum FeedbackMessageType
    {
        Success,
        Info,
        Warning,
        Danger
    }
}
