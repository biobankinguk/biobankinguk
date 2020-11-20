using Directory.Data.Migrations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Biobanks.Web.Models.ADAC
{
    public class SampleCollectionModesModel
    {
        public ICollection<SampleCollectionModeModel> SampleCollectionModes { get; set; }
    }

    public class SampleCollectionModeModel
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        public int SortOrder { get; set; }

        public int SampleSetsCount { get; set; }
    }
}