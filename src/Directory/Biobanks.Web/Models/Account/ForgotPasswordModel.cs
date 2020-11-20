using System.ComponentModel.DataAnnotations;

namespace Biobanks.Web.Models.Account
{
    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "Please enter your e-mail address.")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }
    }
}