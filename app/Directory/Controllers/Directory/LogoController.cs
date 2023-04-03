using System.Threading.Tasks;
using Biobanks.Directory.Services.Directory.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Biobanks.Directory.Controllers.Directory
{
    public class LogoController : Controller
    {
        readonly ILogoService _logoService;
        
        public LogoController(ILogoService logoService)
        {
            _logoService = logoService;
        }

        //Get: Blob
        public async Task<ActionResult> Index(string logoName)
        {
          var blob = await _logoService.GetLogoBlob(logoName);
          
          //filename should have extension, so we don't have to extract it from the content disposition header
          return File(blob.Content, blob.ContentType);
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
