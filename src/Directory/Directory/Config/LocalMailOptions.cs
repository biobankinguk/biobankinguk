namespace Directory.Config
{
    public class LocalMailOptions : EmailSenderOptions
    {
        public string LocalPath { get; set; } = "/temp";
    }
}
