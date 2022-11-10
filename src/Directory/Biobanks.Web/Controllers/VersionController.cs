using Biobanks.Web.HtmlHelpers;
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
    public class VersionController : Controller
    {
        // GET: Version
        public string Index()
        {            
            return Utilities.VersionHelper.GetInformationalVersion();
        }
    }
}