namespace Biobanks.Submissions.Api.Config
{
    public record SiteConfigOptions
    {
        public string ServiceName { get; init; } = "Biobanks";

        public string ContactAddress { get; init; } = string.Empty;

        public string EmailSignature { get; init; } = string.Empty;
    }
}
