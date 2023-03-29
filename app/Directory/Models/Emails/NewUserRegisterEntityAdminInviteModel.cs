namespace Biobanks.Submissions.Api.Models.Emails;

public record NewUserRegisterEntityAdminInviteModel(
  string name, 
  string entity, 
  string confirmLink);
