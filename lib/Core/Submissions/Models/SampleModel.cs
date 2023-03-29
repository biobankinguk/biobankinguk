using System;
using System.ComponentModel.DataAnnotations;
using Biobanks.Submissions.Types;

namespace Biobanks.Submissions.Models
{
    public class SampleModel : SampleIdModel
    {
        /// <summary>
        /// Year of birth, in ISO-8601 YYYY or IETF RFC-3339 date-fullyear format, e.g. 1986
        /// </summary>
        [Range(1000, 9999)]
        public int? YearOfBirth { get; set; }

        [Range(0, 150)]
        public string AgeAtDonation { get; set; }

        /// <summary>
        /// The type of material
        /// </summary>
        [Required]
        public string MaterialType { get; set; }

        /// <summary>
        /// The routine temperature that the sample is kept at
        /// </summary>
        [Required]
        public string StorageTemperature { get; set; }

        /// <summary>
        /// The method in which the sample was preserved, if applicable.
        /// </summary>
        public string PreservationType { get; set; }

        /// <summary>
        /// The date the sample was created, in ISO-8601 extended date or IETF RFC-3339 full-date format, e.g. 2016-01-01
        /// </summary>
        public DateTime? DateCreated { get; set; }

        /// <summary>
        /// The sex of the individual.
        /// </summary>
        public string Sex { get; set; }

        //The properties below are all conditionally nullable

        #region Tissue Sample properties

        public string ExtractionSite { get; set; }
        public string ExtractionSiteOntology { get;set; }
        public string ExtractionSiteOntologyVersion { get;set; }

        public OntologyField ExtractionSiteOntologyField { get; set; } = OntologyField.Code;

        #endregion

        #region Extracted Sample Properties

        public string SampleContent { get; set; }

        public string SampleContentMethod { get; set; }

        public OntologyField SampleContentOntologyField { get; set; } = OntologyField.Code;

        public string ExtractionProcedure { get; set; }

        public OntologyField ExtractionProcedureOntologyField { get; set; } = OntologyField.Code;

        #endregion
    }
}
