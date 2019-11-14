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
        [BindProperty]
        [Required]
        public string Code { get; set; } = string.Empty;

        [BindProperty]
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
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
                return PageWithError("Link", "The User ID or Token is invalid or has expired.");

            UserId = userId; // This is how we get these to React
            Code = code;     // For consistency
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            const string generalError = "The User ID or Token is invalid or has expired.";

            if (!ModelState.IsValid)
            {
                // are the errors with the userid/code or the actual form submission?
                var formErrors = !ModelState.Keys.All(
                    x => new[]
                    {
                        nameof(Code), nameof(UserId)
                    }
                    .Contains(x));

                return formErrors
                    ? Page() // Actual form field errors should go back to the form
                    : PageWithError(ReactRoutes.ResetPasswordResult, string.Empty, generalError);
            }

            if (Password != PasswordConfirm) // Client side should catch this, but we should be on the safe side.
                return PageWithError("The passwords entered do not match.");

            // We need to keep the ViewModel storing the URL safe code, for any page returns that need it
            var rawCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Code));

            await _tokenLog.PasswordResetTokenValidationAttempted(rawCode, UserId);

            var user = await _users.FindByIdAsync(UserId);
            if (user is null)
            {
                await _tokenLog.PasswordResetTokenValidationFailed(
                    rawCode, UserId, JsonSerializer.Serialize(new[] {
                        new { Code = "InvalidUserId", Description = "Invalid User ID" }
                    }));
                return PageWithError(ReactRoutes.ResetPasswordResult, string.Empty, generalError);
            }

            var result = await _users.ResetPasswordAsync(user, rawCode, Password);
            if (!result.Succeeded)
            {
                await _tokenLog.PasswordResetTokenValidationFailed(
                    rawCode, UserId, JsonSerializer.Serialize(result.Errors));

                foreach (var error in result.Errors)
                {
                    if (error.Code == "InvalidToken") // Token only; User is already validated above
                    {
                        ModelState.ClearValidationState(string.Empty);
                        return PageWithError(ReactRoutes.ResetPasswordResult, string.Empty, generalError);
                    }
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return Page();
            }

            await _signIn.SignInAsync(user, false);
            await _tokenLog.PasswordResetTokenValidationSuccessful(rawCode, UserId);

            return Page(ReactRoutes.ResetPasswordResult);
        }
    }
}
