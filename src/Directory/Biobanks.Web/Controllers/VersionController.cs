using Biobanks.Web.HtmlHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Biobanks.Web.Controllers
{
    public class VersionController : Controller
    {
        // GET: Version
        public string Index()
        {            
            return Utilities.VersionHelper.GetInformationalVersion();
        }
    }
}