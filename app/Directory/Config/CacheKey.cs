namespace Biobanks.Submissions.Api.Config
{
    /// <summary>
    /// Keys for cached Data (i.e. NOT cached Site Config values)
    /// </summary>
    public static class CacheKey
    {
        public static string DonorCounts => "data.DonorCounts";

        public static string WordpressNavItems => "data.WordpressNavItems";
    }
}
