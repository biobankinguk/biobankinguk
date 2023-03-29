using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Biobanks.Directory.Utilities
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
    {
      var feedback = new FeedbackMessage
      {
        Message = message,
        Type = type,
        ContainsHtml = containsHtml
      };

      controller.TempData[FeedbackMessageKey.FeedbackMessage] = JsonConvert.SerializeObject(feedback);
    }}

  public static class FeedbackMessageKey
  {
    public static string FeedbackMessage = "TemporaryFeedbackMessage";
  }
}
