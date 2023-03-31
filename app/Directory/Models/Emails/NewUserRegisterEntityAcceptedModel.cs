namespace Biobanks.Directory.Models.Emails;

public record NewUserRegisterEntityAcceptedModel(
  string name,
  string entity,
  string confirmLink);
