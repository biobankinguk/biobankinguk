namespace Directory.Pages.Components.PostLogoutRedirect
{
    public class PostLogoutRedirectViewModel
    {
        public string? PostLogoutRedirectUri { get; set; }
        public string? ClientName { get; set; }
        public string? SignOutIframeUrl { get; set; }

        public string? LogoutId { get; set; }
    }
}
