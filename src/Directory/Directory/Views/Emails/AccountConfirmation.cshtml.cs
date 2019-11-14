namespace Directory.Views.Emails
{
    public class AccountConfirmationModel
    {
        public string Name { get; set; }
        public string Link { get; set; }

        public AccountConfirmationModel(string name, string link)
        {
            Name = name;
            Link = link;
        }
    }
}
