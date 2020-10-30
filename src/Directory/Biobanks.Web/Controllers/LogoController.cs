using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Directory.Services.Contracts;

namespace Biobanks.Web.Controllers
{
    [AllowAnonymous]
    public class LogoController : Controller
    {
        public ActionResult Index(string logoName)
            => RedirectToAction("Index", "Blob", new { resourceName = logoName });

        public ActionResult NoLogo()
            => File("~/Content/images/NoLogo.png", "image/png");

        public ActionResult NoLogoPublic()
            => File("~/Content/images/NoLogoPublic.png", "image/png");
    }
}
