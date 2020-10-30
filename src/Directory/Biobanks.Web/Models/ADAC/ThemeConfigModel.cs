using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Biobanks.Web.Models.ADAC
{
    public class ThemeConfigModel
    {
        // Header Logo
        [DataType(DataType.Upload)]
        public HttpPostedFileBase HeaderLogo { get; set; }

        public string HeaderLogoName { get; set; }

        // Splash Background
        [DataType(DataType.Upload)]
        public HttpPostedFileBase SplashBackground { get; set; }

        public string SplashBackgroundName { get; set; }

        // Footer Logos
        [DataType(DataType.Upload)]
        public IEnumerable<HttpPostedFileBase> FooterLogos { get; set; }

        public IEnumerable<string> FooterLogoNames { get; set; }

        public IEnumerable<string> FooterLinks { get; set; }

        public IEnumerable<bool> RemoveFooterLogos { get; set; }

        // CSS Theme
        [DataType(DataType.Upload)]
        public HttpPostedFileBase CssTheme { get; set; }
    }
}