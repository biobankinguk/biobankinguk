using System.ComponentModel.DataAnnotations;

namespace Biobanks.Submissions.Api.Models.Account
{
    public class AccountDetailsModel
    {
        [Required]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Please enter your name.")]
        public string Name { get; set; }

        //not required because not editable
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
    }
}