namespace Biobanks.Directory.Config
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

    public string AlternateHomepage { get; init; } = "false";

    public bool HotjarEnabled { get; set; }

    // Google Analytics / Recaptcha
    public bool GoogleAnalyticsEnabled { get; set; }

    public string GoogleAnalyticsTrackingCode { get; set; } = string.Empty;

    public string GoogleTagId { get; set; } = string.Empty;

    public string GoogleRecaptchaSecret { get; set; } = string.Empty;
    
    public string GoogleRecaptchaPublicKey { get; set; } = string.Empty;
    
    /// <summary>
    /// Allow users to access their own suspended biobanks.
    /// </summary>
    public bool AllowSuspendedBiobanks { get; set; } = true;

    /// <summary>
    /// How long in milliseconds authenticated user sessions should last on the client side
    /// </summary>
    public int ClientSessionTimeout { get; set; } = 1200000; //20 mins in milliseconds
    
  }
}
