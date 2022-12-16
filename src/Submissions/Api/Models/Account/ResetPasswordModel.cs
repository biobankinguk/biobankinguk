using System.ComponentModel.DataAnnotations;

namespace Biobanks.Submissions.Api.Models.Account
{
    public class ResetPasswordModel
    {

        [Required(ErrorMessage = "Please enter a password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Token { get; set; }
        public string UserId { get; set; }
    }
}