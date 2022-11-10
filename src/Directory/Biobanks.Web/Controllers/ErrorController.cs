using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Biobanks.Web.Controllers
{
    [Obsolete("To be deleted when the Directory core version goes live." +
          " Any changes made here will need to be made in the corresponding core version"
    , false)]
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
            return new FilePathResult("~/Error/error.html", "text/html");
        }
    }
}