using Biobanks.Submissions.Api.Models.Emails;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.EmailServices.Contracts
{
    public interface IEmailService
    {
        Task SendExternalNetworkNonMemberInformation(EmailAddress to, string biobankName, string biobankAnonymousIdentifier, string networkName, string networkContactEmail, string networkDescription);

        Task SendContactList(EmailAddress to, string contactlist, bool contactMe);

        Task SendPasswordReset(EmailAddress to, string username, string resetLink);

        Task ResendAccountConfirmation(EmailAddress email, string name, string action);
  };
}
