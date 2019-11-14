using System;
using System.Threading.Tasks;
using Directory.Views.Emails;

namespace Directory.Services
{
    public class AccountEmailService
    {
        private readonly IEmailSender _emails;

        public AccountEmailService(IEmailSender emails)
        {
            _emails = emails;
        }

        public async Task SendAccountConfirmation(string to, string name, string link)
            => await _emails.SendEmail(
                to,
                "UKCRC Tissue Directory Account Confirmation",
                "Emails/AccountConfirmation",
                new AccountConfirmationModel(name, link),
                name);

        public async Task SendPasswordReset(string to, string name, string link)
            => await _emails.SendEmail(
                to,
                "UKCRC Tissue Directory Password Reset",
                "Emails/PasswordReset",
                new PasswordResetModel(name, link),
                name);
    }
}
