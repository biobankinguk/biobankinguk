using System.ComponentModel.DataAnnotations;

namespace Biobanks.Submissions.Api.Models.Account
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Please enter your email address.")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter your password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
