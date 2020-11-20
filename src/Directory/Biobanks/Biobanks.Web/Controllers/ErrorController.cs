using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Biobanks.Web.Controllers
{
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