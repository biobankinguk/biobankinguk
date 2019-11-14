using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Directory.Auth.Identity;
using Directory.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Directory.Pages.Account
{
    public class ForgotPasswordModel : BaseReactModel
    {
        [BindProperty]
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public bool AllowResend { get; set; }

        private readonly DirectoryUserManager _users;
        private readonly TokenLoggingService _tokenLog;
        private readonly AccountEmailService _accountEmail;

        public ForgotPasswordModel(
            DirectoryUserManager users,
            TokenLoggingService tokenLog,
            AccountEmailService accountEmail)
            : base(ReactRoutes.ForgotPassword)
        {
            _users = users;
            _tokenLog = tokenLog;
            _accountEmail = accountEmail;
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid) return Page();

            var user = await _users.FindByEmailAsync(Email);
            // if no user found, we just pretend like they were, but without actually sending the email

            if (user is { })
            {
                if (!user.EmailConfirmed)
                {
                    AllowResend = true;
                    return Page();
                }

                var code = await _users.GeneratePasswordResetTokenAsync(user);
                var urlCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var link = Url.Page("/Account/ResetPassword",
                    pageHandler: null,
                    values: new { userId = user.Id, code = urlCode },
                    protocol: Request.Scheme);

                await _tokenLog.PasswordResetTokenIssued(code, user.Id);

                await _accountEmail.SendPasswordReset(
                    user.Email,
                    user.Name,
                    link);
            }

            return Page(ReactRoutes.ForgotPasswordResult);
        }
    }
}
