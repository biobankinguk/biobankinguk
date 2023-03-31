namespace Biobanks.Directory.Models.Emails;

public record DirectoryAdminNewRegisterRequestNotificationModel(string RequesterName, string RequesterEmail, string EntityName, string EntityType);
