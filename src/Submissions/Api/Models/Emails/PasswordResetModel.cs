namespace Biobanks.Submissions.Api.Models.Emails;

public record PasswordResetModel(string ResetLink, string Username);

