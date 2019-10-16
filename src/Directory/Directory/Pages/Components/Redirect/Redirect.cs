using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Directory.Pages.Components.Redirect
{
    public class RedirectViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync(string redirectUrl)
            => Task.FromResult<IViewComponentResult>(
                View(new RedirectViewModel { RedirectUrl = redirectUrl }));
    }
}
