using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Biobanks.Web.Models.ADAC
{
    public class RegisterConfigModel
    {
        public string BiobankTitle { get; set; }
        public string BiobankDescription { get; set; }
        public string NetworkTitle { get; set; }
        public string NetworkDescription { get; set; }
    }
}