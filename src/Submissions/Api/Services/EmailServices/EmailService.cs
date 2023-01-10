using Biobanks.Submissions.Api.Config;
using Biobanks.Submissions.Api.Models.Emails;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Services.EmailServices.Contracts;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.EmailServices
{
    public class EmailService : IEmailService
    {
        private readonly SitePropertiesOptions _siteConfig;
        private readonly IEmailSender _emailSender;


        public EmailService(
            IOptions<SitePropertiesOptions> siteConfigOptions,
            IEmailSender emailSender)
        {
            _siteConfig = siteConfigOptions.Value;
            _emailSender = emailSender;
        }

    public async Task SendExternalNetworkNonMemberInformation(EmailAddress to, string biobankName,
            string biobankAnonymousIdentifier, string networkName, string networkContactEmail, string networkDescription)
        {
            await _emailSender.SendEmail(
                to, 
                "Emails/ExternalNetworkNonMemberInformation", 
                new NonMemberEmailModel(
                    to.Name!,
                    biobankName,
                    biobankAnonymousIdentifier,
                    networkName,
                    networkContactEmail,
                    networkDescription)
                );
        }

        public async Task SendContactList(EmailAddress to, string contactlist, bool contactMe)
        {
            EmailAddress ccAddress = null;
            if (contactMe)
            {
                if (string.IsNullOrWhiteSpace(_siteConfig.ContactAddress))
                {
                    ccAddress = new EmailAddress(_siteConfig.ContactAddress);
                }
            }
            await _emailSender.SendEmail(
                   to,
                   "Emails/EmailContactList",
                   new ContactListEmailModel(
                       to.Name!,
                       contactlist
                       ),
                   ccAddress
                   );
        }

    public async Task SendPasswordReset(EmailAddress to, string username, string resetLink)
    {
      await _emailSender.SendEmail(
        to,
        "Emails/PasswordReset",
        new PasswordResetModel(resetLink, username)
        );
    }

    public async Task ResendAccountConfirmation(EmailAddress to, string name, string action)
    {
      await _emailSender.SendEmail(
        to,
        "Emails/ResendConfirm",
        new ResendConfirmModel(name, action));

      throw new System.NotImplementedException();
    }

    public async Task SendNewUserRegisterEntityAdminInvite(EmailAddress to, string name, string entity, string confirmLink)
    {
      await _emailSender.SendEmail(
        to,
        "Emails/NewUserRegisterEntityAdminInvite",
        new NewUserRegisterEntityAdminInviteModel(name, entity, confirmLink)
        );
    }

    public async Task SendExistingUserRegisterEntityAdminInvite(EmailAddress to, string name, string entity, string link)
    {
      await _emailSender.SendEmail(
        to,
        "Emails/ExistingUserRegisterEntityAdminInvite",
        new NewUserRegisterEntityAdminInviteModel(name, entity, link)
        );
    }
    public async Task SendNewUserRegisterEntityAccepted(EmailAddress to, string name, string entity, string confirmLink)
    {
      await _emailSender.SendEmail(
        to,
        "Emails/NewUserRegisterEntityAccepted",
        new NewUserRegisterEntityAcceptedModel(name, entity, confirmLink)
        );
    }

    public async Task SendExistingUserRegisterEntityAccepted(EmailAddress to, string name, string entity, string link)
    {
      await _emailSender.SendEmail(
        to,
        "Emails/ExistingUserRegisterEntityAccepted",
        new ExistingUserRegisterEntityAcceptedModel(name, entity, link)
        );
    }

    public async Task SendRegisterEntityDeclined(EmailAddress to, string name, string entity)
    {
      await _emailSender.SendEmail(
        to,
        "Emails/RegisterEntityDeclined",
        new RegisterEntityDeclinedModel(name, entity)
        );
    }

    public async Task SendNewBiobankRegistrationNotification(EmailAddress to, string biobankName, string networkName, string link)
    {
      await _emailSender.SendEmail(
        to,
        "Emails/NewBiobankNetworkRegistrationNotification",
        new NewBiobankNetworkRegistrationNotificationModel(biobankName, networkName, link)
        );
    }
  }
}
