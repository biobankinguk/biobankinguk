using System;
using System.Text;
using System.Threading.Tasks;
using Common.Data.Identity;
using Directory.Auth.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Directory.Services
{
    /// <summary>
    /// Centralise the issuing of various token types via email
    /// so that can be performed from multiple locations
    /// (e.g. different Controller Actions)
    /// </summary>
    public class TokenIssuingService
    {
        private readonly TokenLoggingService _tokenLog;
        private readonly AccountEmailService _accountEmail;
        private readonly DirectoryUserManager _users;

        public TokenIssuingService(
            DirectoryUserManager users,
            TokenLoggingService tokenLog,
            AccountEmailService accountEmail)
        {
            _tokenLog = tokenLog;
            _accountEmail = accountEmail;
            _users = users;
        }

        public string Scheme { get; set; } = "https";
        public IUrlHelper? Url { get; set; }

        /// <summary>
        /// Issue an AccountConfirmation token, log the issuing event and email the user a link.
        /// </summary>
        /// <param name="user">The user to issue the token for and send the email to.</param>
        public async Task SendAccountConfirmation(DirectoryUser user)
        {
            ValidateUrlHelper();

            var code = await _users.GenerateEmailConfirmationTokenAsync(user);
            await _tokenLog.AccountConfirmationTokenIssued(code, user.Id);

            await _accountEmail.SendAccountConfirmation(
                user.Email,
                user.Name,
                link: Url.Page("/Account/Confirm",
                    pageHandler: null,
                    values: new
                    {
                        userId = user.Id,
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code))
                    },
                    protocol: Scheme));
        }

        /// <summary>
        /// Issue a Password Reset token, log the issuing event and email the user a link.
        /// </summary>
        /// <param name="user">The user to issue the token for and send the email to.</param>
        public async Task SendPasswordReset(DirectoryUser user)
        {
            ValidateUrlHelper();

            var code = await _users.GeneratePasswordResetTokenAsync(user);
            await _tokenLog.PasswordResetTokenIssued(code, user.Id);

            await _accountEmail.SendPasswordReset(
                user.Email,
                user.Name,
                link: Url.Page("/Account/ResetPassword",
                    pageHandler: null,
                    values: new
                    {
                        userId = user.Id,
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code))
                    },
                    protocol: Scheme));
        }

        private void ValidateUrlHelper()
        {
            if (Url is null)
                throw new InvalidOperationException(
                    $@"{nameof(Url)} must be set to a valid {
                        nameof(IUrlHelper)} for the calling context.");
        }

        public TokenIssuingService WithUrlHelper(IUrlHelper url)
        {
            Url = url;
            return this;
        }
    }
}
