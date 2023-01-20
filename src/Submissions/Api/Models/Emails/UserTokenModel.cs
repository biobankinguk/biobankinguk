using System.ComponentModel.DataAnnotations;

namespace Biobanks.Submissions.Api.Models.Emails;

public record UserTokenModel(
    [Required]
    string UserId,
    [Required]
    string Token);

