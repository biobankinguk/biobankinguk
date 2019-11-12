using System.ComponentModel.DataAnnotations;
using Directory.Auth.Identity;
using Directory.Services;
using Microsoft.AspNetCore.Mvc;

namespace Directory.Pages.Account
{
    public class ForgotPasswordModel : BaseReactModel
    {
        [BindProperty]
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public ForgotPasswordModel(
            DirectoryUserManager users,
            TokenLoggingService tokenLog,
            AccountEmailService accountEmail)
            : base(ReactRoutes.ForgotPassword) { }

        public void OnGet()
        {
        }

        public void OnGetReset()
        {
            Route = ReactRoutes.ResetPassword;
            Page();
        }
    }
}
