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
        private readonly SiteConfigOptions _siteConfig;
        private readonly IEmailSender _emailSender;


        public EmailService(
            IOptions<SiteConfigOptions> siteConfigOptions,
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
    }
}
