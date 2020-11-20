using System.ComponentModel.DataAnnotations;

namespace Biobanks.Web.Models.Account
{
    public class ResetPasswordModel
    {

        [Required(ErrorMessage = "Please enter a password.")]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Token { get; set; }
        public string UserId { get; set; }
    }
}