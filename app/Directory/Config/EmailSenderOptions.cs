namespace Biobanks.Directory.Config
{
  public record BaseEmailSenderOptions
  {
    public BaseEmailSenderOptions()
    {
      if (string.IsNullOrWhiteSpace(ReplyToAddress))
        ReplyToAddress = FromAddress;
    }

    public string FromName { get; init; } = "No Reply";
    public string FromAddress { get; init; } = "noreply@example.com";
    public string ReplyToAddress { get; init; } = string.Empty;
  };

  public record LocalDiskEmailOptions : BaseEmailSenderOptions
  {
    public string LocalPath { get; init; } = "~/temp";
  }

  public record SendGridOptions : BaseEmailSenderOptions
  {
    public string SendGridApiKey { get; init; } = string.Empty;
  }
}
