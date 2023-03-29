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

        Task SendNewUserRegisterEntityAdminInvite(EmailAddress email, string name, string entity, string confirmLink);

        Task SendExistingUserRegisterEntityAdminInvite(EmailAddress to, string name, string entity, string link);
       
        Task SendNewUserRegisterEntityAccepted(EmailAddress to, string name, string entity, string confirmLink);
        
        Task SendExistingUserRegisterEntityAccepted(EmailAddress to, string name, string entity, string link);
        
        Task SendRegisterEntityDeclined(EmailAddress to, string name, string entity);
        
        Task SendDirectoryAdminNewRegisterRequestNotification(string requesterName, string requesterEmail, string entityName, string entityType);

        Task SendNewBiobankRegistrationNotification(EmailAddress to, string biobankName, string networkName, string link);

  };
}
