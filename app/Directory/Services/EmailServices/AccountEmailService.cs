using System.Threading.Tasks;
using Biobanks.Directory.Models.Emails;
using Biobanks.Directory.Services.Directory.Contracts;

namespace Biobanks.Directory.Services.EmailServices
{
    public class AccountEmailService
    {
        private readonly IEmailSender _emails;

        public AccountEmailService(IEmailSender emails)
        {
            _emails = emails;
        }

        public async Task SendAccountConfirmation(EmailAddress to, string link, string resendLink)
            => await _emails.SendEmail(
                to,
                "Emails/AccountConfirmation",
                new TokenEmailModel(
                    to.Name!,
                    link,
                    resendLink));

        public async Task SendPasswordReset(EmailAddress to, string link, string resendLink)
            => await _emails.SendEmail(
                to,
                "Emails/PasswordReset",
                new TokenEmailModel(
                    to.Name!,
                    link,
                    resendLink));

        public async Task SendEmailChange(EmailAddress to, string link, string resendLink)
            => await _emails.SendEmail(
                to,
                //$"Confirm your new {_config.ServiceName} Email Address",
                "Emails/EmailChange",
                new TokenEmailModel(
                    to.Name!,
                    link,
                    resendLink));
    }
}
