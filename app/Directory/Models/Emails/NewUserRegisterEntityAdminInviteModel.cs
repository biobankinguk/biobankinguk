namespace Biobanks.Directory.Models.Emails;

public record NewUserRegisterEntityAdminInviteModel(
  string name, 
  string entity, 
  string confirmLink);
