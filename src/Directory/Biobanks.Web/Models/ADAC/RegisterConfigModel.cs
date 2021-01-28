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
        public string EnableRegistrationHelpUrl { get; set; }
        public string RegistrationHelpUrl { get; set; }
        public string RegistrationEmails { set; get; }
    }
}