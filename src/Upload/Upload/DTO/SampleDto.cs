using System;
using System.ComponentModel.DataAnnotations;
using Upload.Common.Types;

namespace Upload.DTO
{
    public class SampleDto : SampleIdDto
    {
        /// <summary>
        /// Year of birth, in ISO-8601 YYYY or IETF RFC-3339 date-fullyear format, e.g. 1986
        /// </summary>
        [Range(1000, 9999)]
        public int? YearOfBirth { get; set; }

        [Range(0, 150)]
        public int? AgeAtDonation { get; set; }

        /// <summary>
        /// The type of material
        /// </summary>
        [Required]
        public string MaterialType { get; set; } = string.Empty;

        /// <summary>
        /// The routine temperature that the sample is kept at
        /// </summary>
        [Required]
        public string StorageTemperature { get; set; } = string.Empty;

        /// <summary>
        /// The date the sample was created, in ISO-8601 extended date or IETF RFC-3339 full-date format, e.g. 2016-01-01
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// The sex of the individual.
        /// </summary>
        public string Sex { get; set; } = string.Empty;

        //The properties below are all conditionally nullable

        #region Tissue Sample properties

        public string ExtractionSite { get; set; } = string.Empty;
        public string ExtractionSiteOntology { get; set; } = string.Empty;
        public string ExtractionSiteOntologyVersion { get; set; } = string.Empty;

        public OntologyField ExtractionSiteOntologyField { get; set; } = OntologyField.Code;

        #endregion

        #region Extracted Sample Properties

        public string ExtractionProcedure { get; set; } = string.Empty;

        public string SampleContent { get; set; } = string.Empty;

        public string SampleContentMethod { get; set; } = string.Empty;

        public OntologyField ExtractionProcedureOntologyField { get; set; } = OntologyField.Code;

        #endregion
    }
}
