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
        private readonly TokenIssuingService _tokens;

        public ForgotPasswordModel(
            DirectoryUserManager users,
            TokenIssuingService tokens)
            : base(ReactRoutes.ForgotPassword)
        {
            _users = users;
            _tokens = tokens;
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

                await _tokens.SendPasswordReset(user, Request.Scheme);
            }

            return Page(ReactRoutes.ForgotPasswordResult);
        }
    }
}
