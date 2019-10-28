using System.Threading.Tasks;
using Common.Data;

namespace Directory.Services
{
    public enum TokenPurpose { AccountConfirmation }

    public enum TokenEvent { Issued, ValidationAttempted, ValidationSuccessful }

    public class TokenLoggingService
    {
        private readonly DirectoryContext _db;

        public TokenLoggingService(DirectoryContext db)
        {
            _db = db;
        }

        private async Task LogEvent(string eventType, string purpose, string token, string userId)
        {
            _db.TokenRecords.Add(new Common.Data.Identity.TokenRecord(purpose, token, userId, eventType));
            await _db.SaveChangesAsync();
        }

        public async Task AccountConfirmationTokenIssued(string token, string userId)
            => await LogEvent(
                nameof(TokenEvent.Issued),
                nameof(TokenPurpose.AccountConfirmation),
                token, userId);

        public async Task AccountConfirmationTokenValidationAttempted(string token, string userId)
            => await LogEvent(
                nameof(TokenEvent.ValidationAttempted),
                nameof(TokenPurpose.AccountConfirmation),
                token, userId);

        public async Task AccountConfirmationTokenValidationSuccessful(string token, string userId)
            => await LogEvent(
                nameof(TokenEvent.ValidationSuccessful),
                nameof(TokenPurpose.AccountConfirmation),
                token, userId);
    }
}
