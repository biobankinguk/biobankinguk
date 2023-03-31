using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Directory.Models.Register;

public class RegisterEntityModel
{
    [Required(ErrorMessage = "Please enter an admin name.")]
    [DisplayName("Admin name")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Please enter an admin email address.")]
    [DataType(DataType.EmailAddress)]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [DisplayName("Admin email")]
    public string Email { get; set; }
    
    public string EntityName { get; set; } //for label
    
    [Required(ErrorMessage = "Please enter a name for the organisation.")]
    public string Entity { get; set; }
    
    public bool AdacInvited { get; set; } //did ADAC make this registration as an invite? it should be auto-accepted
    
    public bool AcceptTerms { get; set; } //Honeypot field
}
