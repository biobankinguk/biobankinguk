using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Biobanks.Web.Models.Home
{
    public class PublicationSearchBoxModel
    {
        public string SearchTitle { get; set; }

        public string SearchSubTitle { get; set; }

        public string SearchRelatedPublications { get; set; }

        public string SearchRelatedBiobanks { get; set; }

    }
}