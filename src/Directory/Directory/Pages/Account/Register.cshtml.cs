using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Directory.Pages.Account
{
    public class RegisterModel : PageModel
    {
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


    }
}