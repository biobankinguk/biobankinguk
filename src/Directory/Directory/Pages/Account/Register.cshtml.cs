using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Common.Data.Identity;
using Directory.Auth.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Directory.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly DirectoryUserManager _users;

        public RegisterModel(DirectoryUserManager users)
        {
            _users = users;
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
                    Name = FullName!, // [Required] prevents null, empty string
                    EmailConfirmed = true // TODO: set to false once we add confirmation process
                };

                var result = await _users.CreateAsync(user, Password);
                if (result.Succeeded)
                {

                    // TODO: Send confirmation email

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