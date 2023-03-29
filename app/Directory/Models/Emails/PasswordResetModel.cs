namespace Biobanks.Directory.Models.Emails;

public record PasswordResetModel(string ResetLink, string Username);

