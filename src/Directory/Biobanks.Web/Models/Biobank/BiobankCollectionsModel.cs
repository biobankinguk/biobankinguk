﻿using System.Collections.Generic;

namespace Biobanks.Web.Models.Biobank
{
    public class BiobankCollectionsModel
    {
         public IEnumerable<BiobankCollectionModel> BiobankCollectionModels { get; set; }
    }

    public class BiobankCollectionModel
    {
        public int Id { get; set; }
        public string SnomedTerm { get; set; }
        public string Title { get; set; }
        public int StartYear { get; set; }
        public string MaterialTypes { get; set; }
        public int NumberOfSampleSets { get; set; }
    }
}