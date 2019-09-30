using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Directory.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool Success;

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (!ModelState.IsValid) return Page();

            Success = true;

            return Page();
        }
    }
}