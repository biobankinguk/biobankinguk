﻿using Biobanks.Directory.Data.Migrations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Biobanks.Web.Models.ADAC
{
    public class MacroscopicAssessmentsModel
    {
        public ICollection<MacroscopicAssessmentModel> MacroscopicAssessments { get; set; }
    }

    public class MacroscopicAssessmentModel
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        public int SortOrder { get; set; }

        public int SampleSetsCount { get; set; }
    }
}