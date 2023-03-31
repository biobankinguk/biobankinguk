namespace Biobanks.Directory.Models.Emails;

public record NewBiobankNetworkRegistrationNotificationModel(
  string biobankName, 
  string networkName, 
  string link);
