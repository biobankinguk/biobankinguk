using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Directory.Pages.Components.Redirect
{
    public class RedirectViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string redirectUrl)
            => View(new RedirectViewModel { RedirectUrl = redirectUrl });
    }
}
