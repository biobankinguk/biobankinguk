using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data.Entities;
using Biobanks.Directory.Config;
using Biobanks.Directory.Constants;
using Biobanks.Directory.Models.Emails;
using Biobanks.Directory.Services.Directory.Contracts;
using Biobanks.Directory.Services.EmailServices.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Biobanks.Directory.Services.EmailServices
{
    public class EmailService : IEmailService
    {
        private readonly SitePropertiesOptions _siteConfig;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmailService(
            IOptions<SitePropertiesOptions> siteConfigOptions,
            IEmailSender emailSender,
            UserManager<ApplicationUser> userManager)
        {
            _siteConfig = siteConfigOptions.Value;
            _emailSender = emailSender;
            _userManager = userManager;
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
            new ExistingUserRegisterEntityAdminInviteModel(name, entity, link)
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
        
        public async Task SendDirectoryAdminNewRegisterRequestNotification(string requesterName, string requesterEmail,  string entityType, string entityName)
        {
            // Get list of Admin users email addresses
            var userEmails = _userManager.GetUsersInRoleAsync(Role.DirectoryAdmin).Result
                .Select(user => user.Email?.Trim() )
                .Where(email => !string.IsNullOrEmpty(email))
                .Select(x => new EmailAddress(x))
                .ToList();

            // Fallback on Admin Support if no Admin users
            if (!userEmails.Any())
            {
                userEmails.Add(new EmailAddress(_siteConfig.SupportAddress));
            }
            
            await _emailSender.SendEmail(
                userEmails,
                "Emails/DirectoryAdminNewRegisterRequestNotification",
                new DirectoryAdminNewRegisterRequestNotificationModel(requesterName, requesterEmail, entityName, entityType)
            );

        }
    
  }
}
