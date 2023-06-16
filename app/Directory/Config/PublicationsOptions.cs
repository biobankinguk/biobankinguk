namespace Biobanks.Directory.Config;

public record PublicationsOptions
{
  public string EpmcApiUrl { get; set; } = "https://www.ebi.ac.uk/europepmc/";
}
