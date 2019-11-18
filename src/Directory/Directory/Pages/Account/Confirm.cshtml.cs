using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Common.Data.Identity;
using Directory.Auth.Identity;
using Directory.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;

namespace Directory.Pages.Account
{
    public class ConfirmModel : BaseReactModel
    {
        public string? Username { get; set; }

        private readonly DirectoryUserManager _users;
        private readonly SignInManager<DirectoryUser> _signIn;
        private readonly TokenIssuingService _tokens;
        private readonly TokenLoggingService _tokenLog;

        public ConfirmModel(
            DirectoryUserManager users,
            SignInManager<DirectoryUser> signIn,
            TokenIssuingService tokens,
            TokenLoggingService tokenLog)
            : base(ReactRoutes.Confirm)
        {
            _users = users;
            _signIn = signIn;
            _tokens = tokens;
            _tokenLog = tokenLog;
        }

        public async Task<IActionResult> OnGet(string? userId, string? code)
        {
            var generalError = "The User ID or Token is invalid or has expired.";

            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
                return PageWithError(generalError);

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            await _tokenLog.AccountConfirmationTokenValidationAttempted(code, userId);

            var user = await _users.FindByIdAsync(userId);
            if (user is null)
            {
                await _tokenLog.AccountConfirmationTokenValidationFailed(
                    code, userId, JsonSerializer.Serialize(new[] {
                        new { Code = "InvalidUserId", Description = "Invalid User ID" }
                    }));
                return PageWithError(generalError);
            }

            Username = user.UserName;

            var result = await _users.ConfirmEmailAsync(user, code);

            if (result.Errors.Any())
            {
                await _tokenLog.AccountConfirmationTokenValidationFailed(
                    code, userId, JsonSerializer.Serialize(result.Errors));

                return PageWithError(generalError);
            }

            await _signIn.SignInAsync(user, false);
            await _tokenLog.AccountConfirmationTokenValidationSuccessful(code, userId);

            return Page();
        }

        public async Task<IActionResult> OnGetResend(string? username)
        {
            Username = username;

            var user = await _users.FindByEmailAsync(username);
            if (user is null)
                return PageWithError("Invalid Username");

            await _tokens.WithUrlHelper(Url).SendAccountConfirmation(user);

            return Page(ReactRoutes.ConfirmResend);
        }
    }
}
