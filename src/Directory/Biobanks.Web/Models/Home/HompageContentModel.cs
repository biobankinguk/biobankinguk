using System;
using System.Collections.Generic;

namespace Biobanks.Web.Models.Home
{
    public class HomepageContentModel
    {
        public string Title { get; set; }
        public string NetworkRegistration { get; set; }

        public string ResourceRegistration { get; set; }

        // Collections & Capabilities Search Box
        public string SearchTitle { get; set; }

        public string SearchSubTitle { get; set; }

        public string RequireSamplesCollected { get; set; }

        public string AccessExistingSamples { get; set; }

        // Publications Search Box

        public string PublicationsSearchTitle { get; set; }
        public string PublicationsSearchSubTitle { get; set; }
        public string SearchRelatedPublications { get; set; }

        public string SearchRelatedBiobanks { get; set; }

    }
}