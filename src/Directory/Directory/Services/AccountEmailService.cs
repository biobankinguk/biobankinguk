using System.Threading.Tasks;
using Directory.Pages.Emails;

namespace Directory.Services
{
    public class AccountEmailService
    {
        private readonly IEmailSender _emails;

        public AccountEmailService(IEmailSender emails)
        {
            _emails = emails;
        }

        public async Task SendAccountConfirmation(string to, string name, string confirmLink)
            => await _emails.SendEmail(
                to,
                "UKCRC Tissue Directory Account Confirmation",
                "Emails/AccountConfirmation",
                new AccountConfirmationModel(name, confirmLink));
    }
}
