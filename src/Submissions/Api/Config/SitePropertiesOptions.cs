namespace Biobanks.Submissions.Api.Config
{
  public record SitePropertiesOptions
  {
    public string ServiceName { get; init; } = "Biobanking Directory";

    public string PageTitle { get; set; } = "Biobanking Directory";

    public string ContactAddress { get; init; } = "contact@example.com";

    public string SupportAddress { get; init; } = "support@example.com";

    public string ContactUsUrl { get; init; } = "https://example.com";

    public string EmailSignature { get; init; } = "Biobanking Directory";

    public string LegalEntity { get; init; } = "Legal Entity";

    public bool HotjarEnabled { get; set; }

    // Google Analytics / Recaptcha
    public bool GoogleAnalyticsEnabled { get; set; }

    public string GoogleAnalyticsTrackingCode { get; set; } = string.Empty;

    public string GoogleTagId { get; set; } = string.Empty;
  }
}