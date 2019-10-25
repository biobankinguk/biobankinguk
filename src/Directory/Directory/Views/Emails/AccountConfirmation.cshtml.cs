namespace Directory.Pages.Emails
{
    public class AccountConfirmationModel
    {
        public string Name { get; set; }
        public string ConfirmLink { get; set; }

        public AccountConfirmationModel(string name, string confirmLink)
        {
            Name = name;
            ConfirmLink = confirmLink;
        }
    }
}
