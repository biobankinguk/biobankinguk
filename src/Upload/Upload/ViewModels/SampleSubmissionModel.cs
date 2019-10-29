using System;
using System.ComponentModel.DataAnnotations;
using Biobanks.Common.Models;
using Upload.Common.Types;

namespace Biobanks.SubmissionApi.Models
{
    /// <inheritdoc />
    public class SampleSubmissionModel : SampleIdModel
    {
        /// <summary>
        /// Year of birth of the donor.
        /// </summary>
        [Range(1000, 9999)]
        public int? YearOfBirth { get; set; }

        /// <summary>
        /// The age of the donor at time of making donation.
        /// </summary>
        [Range(0, 150)]
        public int? AgeAtDonation { get; set; }

        /// <summary>
        /// The material type of the sample donated.
        /// </summary>
        public string MaterialType { get; set; } = string.Empty;

        /// <summary>
        /// The storage temperature of the sample.
        /// </summary>
        public string StorageTemperature { get; set; } = string.Empty;

        /// <summary>
        /// The date the sample was donated/taken.
        /// </summary>
        public DateTime DateCreated { get; set; } = new DateTime();

        /// <summary>
        /// The biological sex of the donor.
        /// </summary>
        public string Sex { get; set; } = string.Empty;

        #region Tissue Sample properties

        /// <summary>
        /// The anatomical location of where the sample was extracted.
        /// </summary>
        public string ExtractionSite { get; set; } = string.Empty;

        /// <summary>
        /// The ontology to which the ExtractionSite code relates.
        /// </summary>
        /// <seealso cref="ExtractionSite"/>
        /// <seealso cref="ExtractionSiteOntologyVersion"/>
        public string ExtractionSiteOntology { get; set; } = string.Empty;

        /// <summary>
        /// The version of the ontology to which the ExtractionSite code relates.
        /// </summary>
        /// <seealso cref="ExtractionSite"/>
        /// <seealso cref="ExtractionSiteOntology"/>
        public string ExtractionSiteOntologyVersion { get; set; } = string.Empty;

        /// <summary>
        /// Ontology field to which the code relates.
        /// TODO: This may be slightly confusing as possible values are code and value, but the field is still called code.
        /// </summary>
        public OntologyField ExtractionSiteOntologyField { get; set; } = OntologyField.Code;

        #endregion

        #region Extracted Sample Properties

        /// <summary>
        /// The procedure by which the sample was extracted.
        /// </summary>
        public string ExtractionProcedure { get; set; } = string.Empty;

        /// <summary>
        /// Ontology field to which the code relates.
        /// TODO: This may be slightly confusing as possible values are code and value, but the field is still called code.
        /// </summary>
        public OntologyField ExtractionProcedureOntologyField { get; set; } = OntologyField.Code;

        /// <summary>
        /// The content of the sample.
        /// </summary>
        public string SampleContent { get; set; } = string.Empty;

        /// <summary>
        /// Ontology field to which the code relates.
        /// TODO: This may be slightly confusing as possible values are code and value, but the field is still called code.
        /// </summary>
        public OntologyField SampleContentOntologyField { get; set; } = OntologyField.Code;

        /// <summary>
        /// The macroscopic/microscopic assessment method of the sample content.
        /// </summary>
        public string SampleContentMethod { get; set; } = string.Empty;

        #endregion
    }
}
