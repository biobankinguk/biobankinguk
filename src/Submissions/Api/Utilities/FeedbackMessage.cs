using Microsoft.AspNetCore.Mvc;

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

    public static class TemporaryFeedbackMessageExtensions
    {
        private static string FeedbackMessageKey = "TemporaryFeedbackMessage";

        public static void SetTemporaryFeedbackMessage(this Controller controller, string message, FeedbackMessageType type, bool containsHtml = false)
            => controller.TempData[FeedbackMessageKey] = new FeedbackMessage
            {
                Message = message,
                Type = type,
                ContainsHtml = containsHtml
            };

    }
}
