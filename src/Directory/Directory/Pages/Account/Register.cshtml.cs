using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Common.Data.Identity;
using Directory.Auth.Identity;
using Directory.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace Directory.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly DirectoryUserManager _users;
        private readonly TokenLoggingService _tokenLog;
        private readonly AccountEmailService _accountEmail;

        public RegisterModel(
            DirectoryUserManager users,
            TokenLoggingService tokenLog,
            AccountEmailService _accountEmail)
        {
            _users = users;
            _tokenLog = tokenLog;
            this._accountEmail = _accountEmail;
        }

        public string? Route { get; set; }

        [BindProperty]
        [Required]
        [DataType(DataType.Text)]
        public string? FullName { get; set; }

        [BindProperty]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [BindProperty]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string? EmailConfirm { get; set; }

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        public string? PasswordConfirm { get; set; }

        public void OnGet() => Page();

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid) // Perform additional Model validation
            {
                // Client side should catch these, but we should be on the safe side.
                if (Email != EmailConfirm)
                    ModelState.AddModelError("", "The email addresses entered do not match.");
                if (Password != PasswordConfirm)
                    ModelState.AddModelError("", "The passwords entered do not match.");
            }

            if (ModelState.IsValid) // Actual success route
            {
                var user = new DirectoryUser
                {
                    UserName = Email,
                    Email = Email,
                    Name = FullName! // [Required] prevents null, empty string
                };

                var result = await _users.CreateAsync(user, Password);
                if (result.Succeeded)
                {
                    var code = await _users.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var confirmLink = Url.Page("/Account/Confirm",
                        pageHandler: null,
                        values: new { userId = user.Id, code },
                        protocol: Request.Scheme);

                    await _tokenLog.AccountConfirmationTokenIssued(code, user.Id);

                    await _accountEmail.SendAccountConfirmation(
                        user.Email!, // [Required]
                        user.Name,
                        confirmLink);

                    Route = "register-result";
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return Page();
        }
    }
}