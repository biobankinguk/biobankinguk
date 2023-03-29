namespace Biobanks.Submissions.Api.Auth
{
    /// <summary>
    /// Custom Types used for Claims
    /// </summary>
    public static class CustomClaimTypes
    {
        /// <summary>
        /// Biobank Internal ID that the claim represents access to
        /// </summary>
        public const string BiobankId = "BiobankId";

        /// <summary>
        /// Claim representing the API Client ID of the holder
        /// </summary>
        public const string ClientId = "ClientId";
        
        public const string FullName = "FullName";
        public const string Email = "Email";

    }
}
