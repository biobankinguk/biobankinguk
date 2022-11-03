using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biobanks.Submissions.Api.Controllers.Directory;

[AllowAnonymous]
public class ErrorController : Controller
{
    // GET: Error
    public ActionResult CSRFToken()
    {
        return View();
    }

    public ActionResult Throw()
    {
        return View("Error");
    }

    public ActionResult ThrowGlobal()
    {
        return View("GlobalErrors");
    }

    public ActionResult ThrowStatic()
    {
        return new PhysicalFileResult("~/Error/error.html", "text/html");
    }
}
