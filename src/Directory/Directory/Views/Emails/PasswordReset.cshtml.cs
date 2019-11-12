namespace Directory.Views.Emails
{
    public class PasswordResetModel
    {
        public PasswordResetModel(string name, string link)
        {
            Name = name;
            Link = link;
        }

        public string Name { get; }
        public string Link { get; }
    }
}
