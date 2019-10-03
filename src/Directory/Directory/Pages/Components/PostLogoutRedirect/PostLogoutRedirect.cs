using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Directory.Pages.Components.PostLogoutRedirect
{
    public class PostLogoutRedirectViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(
            string? logoutId,
            string? clientName,
            string? redirectUri,
            string? iframeUrl)
            => View(new PostLogoutRedirectViewModel
            {
                LogoutId = logoutId,
                ClientName = clientName,
                PostLogoutRedirectUri = redirectUri,
                SignOutIframeUrl = iframeUrl
            });
    }
}
