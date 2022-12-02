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
        public static void SetTemporaryFeedbackMessage(this Controller controller, string message, FeedbackMessageType type, bool containsHtml = false)
            => controller.TempData[FeedbackMessageKey.FeedbackMessage] = new FeedbackMessage
            {
                Message = message,
                Type = type,
                ContainsHtml = containsHtml
            };
    }

  public static class FeedbackMessageKey
  {
    public static string FeedbackMessage = "TemporaryFeedbackMessage";
  }
}
