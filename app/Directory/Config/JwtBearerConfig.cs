namespace Biobanks.Directory.Config
{
    /// <summary>
    /// Configuration Options class for JWT Bearer Token usage
    /// </summary>
    public class JwtBearerConfig
    {
        /// <summary>
        /// The Secret used to sign / verify JWTs
        /// </summary>
        public string Secret { get; set; }
    }
}
