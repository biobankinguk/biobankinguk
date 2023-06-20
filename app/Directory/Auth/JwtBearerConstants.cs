namespace Biobanks.Directory.Auth
{
    /// <summary>
    /// Constant values used for configuring JWT Bearer Auth
    /// </summary>
    public static class JwtBearerConstants
    {
        /// <summary>
        /// The token issuer
        /// </summary>
        public const string TokenIssuer = "biobankinguk-api";

        /// <summary>
        /// The intended token audience
        /// </summary>
        public const string TokenAudience = TokenIssuer;
    }
}
