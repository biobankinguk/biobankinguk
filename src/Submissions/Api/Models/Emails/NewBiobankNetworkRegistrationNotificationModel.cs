namespace Biobanks.Submissions.Api.Models.Emails;

public record NewBiobankNetworkRegistrationNotificationModel(
  string biobankName, 
  string networkName, 
  string link);
