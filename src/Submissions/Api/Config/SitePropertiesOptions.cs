namespace Biobanks.Submissions.Api.Config
{
    public record SitePropertiesOptions
    {
        public string ServiceName { get; init; } = "Biobanks";

        public string ContactAddress { get; init; } = string.Empty;

        public string EmailSignature { get; init; } = string.Empty;

        public string LegalEntity { get; init; } = string.Empty;

    }
}
