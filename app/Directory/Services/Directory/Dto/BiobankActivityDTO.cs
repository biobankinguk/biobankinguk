using System;

namespace Biobanks.Submissions.Api.Services.Directory.Dto;

public class BiobankActivityDTO
{
  public int OrganisationId { get; set; }
  public string Name { get; set; }
  public string ContactEmail { get; set; }
  public DateTime? LastUpdated { get; set; }
  public DateTime? LastCapabilityUpdated { get; set; }
  public DateTime? LastCollectionUpdated { get; set; }
  public DateTime? LastAdminLoginTime { get; set; }
  public string LastAdminLoginEmail { get; set; }
}
