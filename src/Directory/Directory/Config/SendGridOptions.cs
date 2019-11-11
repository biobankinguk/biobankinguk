namespace Directory.Config
{
    public class SendGridOptions : EmailSenderOptions
    {
        public string SendGridApiKey { get; set; } = string.Empty;
    }
}
