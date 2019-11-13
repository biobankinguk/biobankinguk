using System.Threading.Tasks;
using Common.Data;

namespace Directory.Services
{
    public enum TokenPurpose { AccountConfirmation, PasswordReset }

    public enum TokenEvent { Issued, ValidationAttempted, ValidationSuccessful, ValidationFailed }

    public class TokenLoggingService
    {
        private readonly DirectoryContext _db;

        public TokenLoggingService(DirectoryContext db)
        {
            _db = db;
        }

        private async Task LogEvent(string eventType, string purpose, string token, string userId, string? details = null)
        {
            _db.TokenRecords.Add(
                new Common.Data.Identity.TokenRecord(purpose, token, userId, eventType)
                {
                    Details = details
                });
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

        public async Task AccountConfirmationTokenValidationFailed(string token, string userId, string? errors = null)
            => await LogEvent(
                nameof(TokenEvent.ValidationFailed),
                nameof(TokenPurpose.AccountConfirmation),
                token, userId,
                errors);

        public async Task PasswordResetTokenIssued(string token, string userId)
            => await LogEvent(
                nameof(TokenEvent.Issued),
                nameof(TokenPurpose.PasswordReset),
                token, userId);

        public async Task PasswordResetTokenValidationAttempted(string token, string userId)
            => await LogEvent(
                nameof(TokenEvent.ValidationAttempted),
                nameof(TokenPurpose.PasswordReset),
                token, userId);

        public async Task PasswordResetTokenValidationSuccessful(string token, string userId)
            => await LogEvent(
                nameof(TokenEvent.ValidationSuccessful),
                nameof(TokenPurpose.PasswordReset),
                token, userId);

        public async Task PasswordResetTokenValidationFailed(string token, string userId, string? errors = null)
            => await LogEvent(
                nameof(TokenEvent.ValidationFailed),
                nameof(TokenPurpose.PasswordReset),
                token, userId,
                errors);
    }
}
