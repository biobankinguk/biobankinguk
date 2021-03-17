namespace Biobanks.Submissions.Api.Auth
{
    /// <summary>
    /// Constant values used for configuring JWT Bearer Auth
    /// </summary>
    public static class JwtBearerConstants
    {
        /// <summary>
        /// The token issuer
        /// </summary>
        public const string TokenIssuer = "biobankinguk-submissions-api";

        /// <summary>
        /// The intended token audience
        /// </summary>
        public const string TokenAudience = TokenIssuer;
    }
}
