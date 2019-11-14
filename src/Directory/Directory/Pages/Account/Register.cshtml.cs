using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Common.Data.Identity;
using Directory.Auth.Identity;
using Directory.Services;
using Microsoft.AspNetCore.Mvc;

namespace Directory.Pages.Account
{
    public class RegisterModel : BaseReactModel
    {
        private readonly DirectoryUserManager _users;
        private readonly TokenIssuingService _tokens;

        public RegisterModel(
            DirectoryUserManager users,
            TokenIssuingService tokens)
            : base(ReactRoutes.Register)
        {
            _users = users;
            _tokens = tokens;
        }

        [BindProperty]
        [Required]
        [DataType(DataType.Text)]
        public string FullName { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        [EmailAddress]
        public string EmailConfirm { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; } = string.Empty;

        public bool AllowResend { get; set; }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid) // Perform additional Model validation
            {
                // Client side should catch these, but we should be on the safe side.
                if (Email != EmailConfirm)
                    ModelState.AddModelError(string.Empty, "The email addresses entered do not match.");
                if (Password != PasswordConfirm)
                    ModelState.AddModelError(string.Empty, "The passwords entered do not match.");
            }

            if (ModelState.IsValid) // Actual success route
            {
                var user = new DirectoryUser
                {
                    UserName = Email,
                    Email = Email,
                    Name = FullName
                };

                var result = await _users.CreateAsync(user, Password);
                if (result.Succeeded)
                {
                    await _tokens.SendAccountConfirmation(user, Request.Scheme);

                    return Page(ReactRoutes.RegisterResult);
                }

                foreach (var error in result.Errors)
                {
                    if (error.Code == "DuplicateEmail")
                    {
                        var existingUser = await _users.FindByEmailAsync(Email);
                        if (!existingUser.EmailConfirmed) AllowResend = true;
                    }
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }
    }
}
