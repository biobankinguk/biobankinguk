using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Common.Data.Identity;
using Directory.Auth.Identity;
using Directory.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace Directory.Pages.Account
{
    public class ConfirmModel : BaseReactModel
    {
        public string? Username { get; set; }

        private readonly DirectoryUserManager _users;
        private readonly TokenLoggingService _tokenLog;
        private readonly SignInManager<DirectoryUser> _signIn;
        private readonly AccountEmailService _accountEmail;

        public ConfirmModel(
            DirectoryUserManager users,
            SignInManager<DirectoryUser> signIn,
            TokenLoggingService tokenLog,
            AccountEmailService accountEmail)
            : base(ReactRoutes.Confirm)
        {
            _users = users;
            _tokenLog = tokenLog;
            _signIn = signIn;
            _accountEmail = accountEmail;
        }

        public async Task<IActionResult> OnGet(string? userId, string? code)
        {
            var generalError = "The User ID or Token is invalid or has expired.";

            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                ModelState.AddModelError(string.Empty, generalError);
                return Page();
            }

            await _tokenLog.AccountConfirmationTokenValidationAttempted(code, userId);

            var user = await _users.FindByIdAsync(userId);
            if (user is null)
            {
                await _tokenLog.AccountConfirmationTokenValidationFailed(
                    code, userId, JsonSerializer.Serialize(new[] {
                        new { Code = "InvalidUserId", Description = "Invalid User ID" }
                    }));

                ModelState.AddModelError(string.Empty, generalError);
                return Page();
            }

            Username = user.UserName;

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            var result = await _users.ConfirmEmailAsync(user, code);

            if (result.Errors.Any())
            {
                await _tokenLog.AccountConfirmationTokenValidationFailed(
                    code, userId, JsonSerializer.Serialize(result.Errors));

                ModelState.AddModelError(string.Empty, generalError);
            }
            else
            {
                await _signIn.SignInAsync(user, false);
                await _tokenLog.AccountConfirmationTokenValidationSuccessful(code, userId);
            }

            return Page();
        }

        public async Task<IActionResult> OnGetResend(string username)
        {
            Username = username;

            var user = await _users.FindByEmailAsync(username);
            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Invalid Username");
                return Page();
            }

            var code = await _users.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var confirmLink = Url.Page("/Account/Confirm",
                pageHandler: null,
                values: new { userId = user.Id, code },
                protocol: Request.Scheme);

            await _tokenLog.AccountConfirmationTokenIssued(code, user.Id);

            await _accountEmail.SendAccountConfirmation(
                user.Email,
                user.Name,
                confirmLink);

            Route = ReactRoutes.ConfirmResend;
            return Page();
        }
    }
}
