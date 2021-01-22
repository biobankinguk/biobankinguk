using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Biobanks.Directory.Services.Contracts;

namespace Biobanks.Web.Controllers
{
    [AllowAnonymous]
    public class LogoController : Controller
    {
        IBiobankReadService _biobankReadService;

        public LogoController(IBiobankReadService biobankReadService)
        {
            _biobankReadService = biobankReadService;
        }

        // GET: Blob
        public async Task<ActionResult> Index(string logoName)
        {
            try
            {
                var blob = await _biobankReadService.GetLogoBlobAsync(logoName);

                //filename should have extension, so we don't have to extract it from the content disposition header
                return File(blob.Content, blob.ContentType);
            }
            catch (ApplicationException)
            {
                throw new HttpException(404, "The requested logo resource could not be found.");
            }
        }

        public ActionResult NoLogo()
        {
            return File("~/Content/images/NoLogo.png", "image/png");
        }

        public ActionResult NoLogoPublic()
        {
            return File("~/Content/images/NoLogoPublic.png", "image/png");
        }
    }
}
