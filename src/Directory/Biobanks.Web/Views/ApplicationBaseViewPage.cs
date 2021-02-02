using System.Security.Claims;
using System.Web.Mvc;
using Biobanks.Identity;

namespace Biobanks.Web.Views
{
    public abstract class ApplicationBaseViewPage<TModel> : WebViewPage<TModel>
    {
        protected ApplicationUserPrincipal CurrentUser => new ApplicationUserPrincipal(User as ClaimsPrincipal);
    }

    public abstract class ApplicationBaseViewPage : ApplicationBaseViewPage<dynamic> { }
}
