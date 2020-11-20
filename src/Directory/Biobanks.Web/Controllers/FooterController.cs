using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Hosting;
using Directory.Data.Caching;
using Newtonsoft.Json;
using Biobanks.Web.Models.Footer;

namespace Biobanks.Web.Controllers
{
    [AllowAnonymous]
    public class FooterController : Controller
    {
        private readonly string _navPath = HostingEnvironment.MapPath(@"~/App_Config/footer.json");

        public ActionResult Footer()
        {
            var json = System.IO.File.ReadAllText(_navPath);
            var model = JsonConvert.DeserializeObject<FooterModel>(json);

            return PartialView("_BBFooter", model);
        }
    }
}