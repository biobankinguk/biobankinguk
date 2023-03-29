namespace Core.Submissions.Types
{
    /// <summary>
    /// Standard parameters for a paginated collection
    /// </summary>
    public class PaginationParams
    {

        /// <summary>
        /// Where to start the page of results.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// How many results to include in a page.
        /// Defaults to 10.
        /// </summary>
        public int Limit { get; set; } = 10;
    }
}
