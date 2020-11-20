namespace Directory.Data.Transforms.Url
{
    public static class UrlSchemes
    {
        public static string Default => Http;

        public static string Http => "http";
        public static string Https => "https";

        public static string AsPrefix(string scheme) => $"{scheme}://";
        public static string AsUrlPrefix(this string scheme) => $"{scheme}://"; //extension method version :)
    }
}
