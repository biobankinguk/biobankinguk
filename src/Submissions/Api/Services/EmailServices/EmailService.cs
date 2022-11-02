using Biobanks.Submissions.Api.Config;
using Biobanks.Submissions.Api.Models.Emails;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.EmailServices
{
    public class EmailService
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

/*
 
public Task SendContactList(string to, string contactlist, bool contactMe)
    {
        dynamic email = new Email("EmailContactList"); // replace with a new one, dynamic  to send email mothod thats like simun, view instead of dynamic and 
        email.To = to;
        email.ContactList = contactlist; // create contactlist model 

        if (contactMe)
            email.Cc = ConfigurationManager.AppSettings["EmailContactAddress"]; // add optional parameter in the method

        _emailSender.SendEmail(email);
    }



public async Task SendPasswordReset(string to, string username, string resetLink)
{
    dynamic email = new Email("PasswordReset");
    email.To = to;
    email.Username = username;
    email.ResetLink = resetLink;
    await SendEmailAsync(email);
}

public async Task SendPasswordReset(EmailAddress to, string link, string resendLink)
    => await _emails.SendEmail(
        to,
        "Emails/PasswordReset",
        new TokenEmailModel(
            to.Name!,
            link,
            resendLink));

*/