namespace Biobanks.Submissions.Api.Models.Emails;

public record NewUserRegisterEntityAcceptedModel(
  string name,
  string entity,
  string confirmLink);
