using System;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Submissions.Dto
{
    public class SampleDto : SampleIdDto
    {
        [Range(1000, 9999)]
        public int? YearOfBirth { get; set; }

        [Range(0, 150)]
        public string AgeAtDonation { get; set; }

        [Required]
        public string MaterialType { get; set; }


        [Required]
        public string StorageTemperature { get; set; }

        public string PreservationType { get; set; }

        public DateTime DateCreated { get; set; }

        public string Sex { get; set; }

        //The properties below are all conditionally nullable

        #region Tissue Sample properties

        public string ExtractionSite { get; set; }

        public string ExtractionSiteOntology { get; set; }
        public string ExtractionSiteOntologyVersion { get; set; }
        public string ExtractionSiteOntologyField { get; set; }

        #endregion

        #region Extracted Sample Properties

        public string ExtractionProcedure { get; set; }
        public string ExtractionProcedureOntology { get; set; }
        public string ExtractionProcedureOntologyVersion { get; set; }
        public string ExtractionProcedureOntologyField { get; set; }

        public string SampleContent { get; set; }
        public string SampleContentOntology { get; set; }
        public string SampleContentOntologyVersion { get; set; }
        public string SampleContentOntologyField { get; set; }
        public string SampleContentMethod { get; set; }

        #endregion
    }
}
