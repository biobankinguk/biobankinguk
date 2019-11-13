using System.ComponentModel.DataAnnotations;
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

namespace Directory.Pages.Account
{
    public class ResetPasswordModel : BaseReactModel
    {
        [Required]
        public string Code { get; set; } = string.Empty;

        [Required]
        public string UserId { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; } = string.Empty;

        private readonly DirectoryUserManager _users;
        private readonly TokenLoggingService _tokenLog;
        private readonly SignInManager<DirectoryUser> _signIn;

        public ResetPasswordModel(
            DirectoryUserManager users,
            SignInManager<DirectoryUser> signIn,
            TokenLoggingService tokenLog)
            : base(ReactRoutes.ResetPassword)
        {
            _users = users;
            _tokenLog = tokenLog;
            _signIn = signIn;
        }

        public IActionResult OnGet(string userId, string code)
        {
            const string linkErrorKey = "Link";
            const string generalError = "The User ID or Token is invalid or has expired.";

            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                ModelState.AddModelError(linkErrorKey, generalError);
                return Page();
            }

            UserId = userId;
            Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            const string generalError = "The User ID or Token is invalid or has expired.";

            if (!ModelState.IsValid)
            {
                // are the errors with the userid/code or the actual form submission?

                if (ModelState.Keys.Any(x =>
                    new[] { nameof(Code), nameof(UserId) }
                    .Contains(x)))
                {
                    // head to the results page with a generic error
                    ModelState.AddModelError(string.Empty, generalError);
                    Route = ReactRoutes.ResetPasswordResult;
                    return Page();
                }

                // Actual form field errors should go back to the form
                return Page();
            }

            if (Password != PasswordConfirm) // Client side should catch this, but we should be on the safe side.
            {
                ModelState.AddModelError(string.Empty, "The passwords entered do not match.");
                return Page();
            }

            await _tokenLog.PasswordResetTokenValidationAttempted(Code, UserId);

            var user = await _users.FindByIdAsync(UserId);
            if (user is null)
            {
                await _tokenLog.PasswordResetTokenValidationFailed(
                    Code, UserId, JsonSerializer.Serialize(new[] {
                        new { Code = "InvalidUserId", Description = "Invalid User ID" }
                    }));

                ModelState.AddModelError(string.Empty, generalError);
                Route = ReactRoutes.ResetPasswordResult;
                return Page();
            }

            var result = await _users.ResetPasswordAsync(user, Code, Password);

            if (result.Errors.Any())
            {
                await _tokenLog.PasswordResetTokenValidationFailed(
                    Code, UserId, JsonSerializer.Serialize(result.Errors));

                ModelState.AddModelError(string.Empty, generalError);
            }
            else
            {
                await _signIn.SignInAsync(user, false);
                await _tokenLog.PasswordResetTokenValidationSuccessful(Code, UserId);
            }

            Route = ReactRoutes.ResetPasswordResult;
            return Page();
        }
    }
}
