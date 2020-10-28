using System;
using System.ComponentModel.DataAnnotations;
using Biobanks.Common.Contracts;

namespace Biobanks.SubmissionJob.Dtos
{
    public class DiagnosisDto : ISubmissionTimestamped, IOrganisationOwnedDto
    {
        public DateTimeOffset SubmissionTimestamp { get; set; }

        public int OrganisationId { get; set; }

        [Required]
        public string IndividualReferenceId { get; set; }

        public DateTime DateDiagnosed { get; set; }

        [Required]
        public string DiagnosisCode { get; set; }

        public string DiagnosisCodeOntology { get; set; }
        public string DiagnosisCodeOntologyVersion { get; set; }
        public string DiagnosisCodeOntologyField { get; set; }
    }
}