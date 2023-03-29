namespace Biobanks.Submissions.Api.Config;

public record AnnualStatisticsOptions 
{
  public string AnnualStatsStartYear { get; init; } = string.Empty;
}
