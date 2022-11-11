using Biobanks.Web.HtmlHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Biobanks.Web.Controllers
{
    [Obsolete("To be deleted when the Directory core version goes live." +
              " We are not porting this controller to core - as this is handled by Version Middleware"
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