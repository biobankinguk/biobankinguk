using Biobanks.Submissions.Api.Models.Footer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System.IO;

namespace Biobanks.Submissions.Api.Controllers.Submissions
{
    [AllowAnonymous]
    public class FooterController : Controller
    {
        private IWebHostEnvironment _hostEnvironment;

        public FooterController(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public ActionResult Footer()
        {
            var _navPath = Path.Combine(_hostEnvironment.WebRootPath, @"~/App_Config/footer.json");
            var json = System.IO.File.ReadAllText(_navPath);
            var model = JsonConvert.DeserializeObject<FooterModel>(json);

            return PartialView("_BBFooter", model);
        }
    }
}
