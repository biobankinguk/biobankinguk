using System.Threading.Tasks;

namespace Directory.Services.Contracts
{
    public interface IEmailService
    {
        Task SendDirectoryAdminNewRegisterRequestNotification(string requesterName, string requesterEmail, string entityName, string entityType);

        Task SendPasswordReset(string to, string username, string resetLink);

        Task SendNewUserRegisterEntityAdminInvite(string to, string name, string entity, string confirmLink);
        Task SendNewBiobankRegistrationNotification(string to, string biobankName, string networkName, string link);

        Task SendNewUserRegisterEntityAccepted(string to, string name, string entity, string confirmLink);

        Task SendExistingUserRegisterEntityAdminInvite(string to, string name, string entity, string link);

        Task SendExistingUserRegisterEntityAccepted(string to, string name, string entity, string link);

        Task SendRegisterEntityDeclined(string to, string name, string entity);

        Task ResendAccountConfirmation(string email, string name, string action);

        Task SendExternalNetworkNonMemberInformation(string to, string biobankName, string biobankAnonymousIdentifier, string networkName, string networkContactEmail, string networkDescription);
        
        Task SendContactList(string to, string contactlist, bool contactMe);
    }
}
