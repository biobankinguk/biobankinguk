using System.Threading.Tasks;

namespace Biobanks.Directory.Services.Directory.Contracts
{
    public interface ITokenLoggingService
    {
        //Validation records get created and return an id, so you can update success status

        Task<int> TokenValidated(string token, string userId, string purpose);
        Task<int> EmailTokenValidated(string token, string userId);
        Task<int> PasswordTokenValidated(string token, string userId);

        Task ValidationSuccessful(int validationRecordId);

        //Issue records are fire and forget; they never need updating
        Task TokenIssued(string token, string userId, string purpose);
        Task EmailTokenIssued(string token, string userId);
        Task PasswordTokenIssued(string token, string userId);
    }
}

