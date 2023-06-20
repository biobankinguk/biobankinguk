namespace Biobanks.Directory.Config
{
    /// <summary>
    /// Options for configuring the API's Hangfire instance
    /// </summary>
    public class HangfireOptions
    {
        /// <summary>
        /// The name of the schema to use for this app's Hangfire tables
        /// </summary>
        public string SchemaName { get; set; } = "Hangfire";
    }
}
